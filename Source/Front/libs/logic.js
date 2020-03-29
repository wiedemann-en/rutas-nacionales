MapHelper = (function ($) {
    'use strict';

    var settings = {
        center: [-38.8116249, -64.7353606],
		cornerSouth: [-56.139851, -76.768524], //[-55.0555521,-76.3060267],
		cornerNorth: [-20.992332, -52.603189], //[-21.6680147,-53.4269817],
        zoom: 4,
		minZoom: 4,
		maxZoom: 12,
		zoomSnap: 0.5,
		zoomDelta: 0.5,
		bounceAtZoomLimits: false
    };

    var mapId = '';
    var map = null;
	var mapboxGL = null;
    var baseMaps = {};
    var overlayMaps = {};
    var routingControl = null;
	var controlsOptions = [];
	var controlsInfo = [];
	var controlsMap = [];
	var controlsHover = [];
	var mapBounds = null;

	var getControlOptions = function (key) {
		for	(let iPos=0; iPos<controlsOptions.length; iPos++) {
			if (controlsOptions[iPos].key == key) {
				return controlsOptions[iPos];
			}
		}
		return null;
	}

	var getControlInfo = function (key) {
		for	(let iPos=0; iPos<controlsInfo.length; iPos++) {
			if (controlsInfo[iPos].IdTramo == key) {
				return controlsInfo[iPos];
			}
		}
		return null;
	}

	var getControlMap = function (key) {
		for	(let iPos=0; iPos<controlsMap.length; iPos++) {
			if (controlsMap[iPos].key == key) {
				return controlsMap[iPos];
			}
		}
		return null;
	}

	var removeControlMap = function (key) {
		for	(let iPos=0; iPos<controlsMap.length; iPos++) {
			if (controlsMap[iPos].key == key) {
				controlsMap.splice(iPos, 1);
				break;
			}
		}
	}

	var getControlHover = function (key) {
		for	(let iPos=0; iPos<controlsHover.length; iPos++) {
			if (controlsHover[iPos].key == key) {
				return controlsHover[iPos];
			}
		}
		return null;
	}

	var removeControlHover = function (key) {
		for	(let iPos=0; iPos<controlsHover.length; iPos++) {
			if (controlsHover[iPos].key == key) {
				controlsHover.splice(iPos, 1);
				break;
			}
		}
	}

    var init = function (mapLayerId, options) {
        settings = $.extend(settings, options);
        mapId = mapLayerId;
        initMap();
    };

    var getMap = function () {
        return map;
    };

    var addRoutingControl = function (routeInfo, fnBallBackSegmentClick) { 
		let waypoints = [];
		$.each(routeInfo.Coordenadas, function (index, coodinate) {
			var itemToAdd = new L.Routing.waypoint(new L.latLng(coodinate.Latitud, coodinate.Longitud));
			waypoints.push(itemToAdd);
		});
		let controlOptionsToAdd = {
			waypoints: waypoints,
			router: new L.Routing.OSRMv1({
				serviceUrl: 'http://localhost:1402/tramos/routing',
				timeout: 30 * 1000,
				profile: 'driving',
				routingOptions: {
					alternatives: false,
					steps: false
				},				
				polylinePrecision: 5,
				useHints: false,
				suppressDemoServerWarning: false,
				language: 'en'				
			}),
			lineOptions: {
				styles: [ { color: routeInfo.ColorRuta, opacity: 0.7, weight: 4, controlKey: routeInfo.IdTramo } ],
				addWaypoints: false,
			},
			autoRoute: true,
			useZoomParameter: false,
			showAlternatives: false,
			tapTolerance: 50,
			fitSelectedRoutes: false, //'smart' -> solo si hace falta //true -> para que ajuste siempre la vista //false -> para que nunca se ajuste
			routeWhileDragging: false,
			routeLine: function(route, options) {
				var line = L.polyline(route.coordinates, options.styles[0]);
				line.on('mouseover', function(e) {
					this.setText('►', {
						repeat: true,
						attributes: {
							fill: 'red'
						}
					});
				});
				line.on('mouseout', function(e) {
					this.setText(null);
				});
				line.on('click', function(e) {
					let controlInfo = getControlInfo(e.target.options.controlKey);
					fnBallBackSegmentClick(controlInfo);
				});
				return line;
			}
		};
		controlOptionsToAdd.key = routeInfo.IdTramo;
		controlOptionsToAdd.visible = false;
		controlsOptions.push(controlOptionsToAdd);
		controlsInfo.push(routeInfo);
	};

	var controlIsMarked = function (key) {
		let controlHover = getControlHover(key);
		let isMarked = (controlHover != null);
		return isMarked;
	};

    var markRoutingControl = function (key) {
		let controlHover = getControlHover(key);
		if (controlHover == null) {
			let controlOptions = getControlOptions(key);
			let clonedOptions = jQuery.extend(true, {}, controlOptions);
			clonedOptions.lineOptions.styles = [
				{ color: 'blue', opacity: 0.4, weight: 8 }, // Shadow
				{ color: 'white', opacity: 0.6, weight: 2 }, // Outline
			];
			let controlHoverToAdd = L.Routing.control(clonedOptions).addTo(map);
			controlHoverToAdd.key = key;
			controlsHover.push(controlHoverToAdd);
		}
    };

    var unmarkRoutingControl = function (key) {
		let controlHover = getControlHover(key);
		if (controlHover != null) {
			controlHover.setWaypoints([]);
			map.removeControl(controlHover);
			removeControlHover(key);
		}
    };

    var unmarkAllRoutingControl = function () {
		$.each(controlsHover, function (index, controlHover) {
			controlHover.setWaypoints([]);
			map.removeControl(controlHover);
		});
		controlsHover = [];
    };

	var showRoutingControls = function (keys) {
		$.each(controlsOptions, function (index, controlOptions) {
			let controlMap = getControlMap(controlOptions.key);
			controlOptions.visible = ($.inArray(controlOptions.key, keys) >= 0);
			if ((controlOptions.visible) && (controlMap == null)) {
				let controlMapToAdd = L.Routing.control(controlOptions).addTo(map);
				controlMapToAdd.key = controlOptions.key;
				controlsMap.push(controlMapToAdd);
			}
			else if ((!controlOptions.visible) && (controlMap != null)) {
				map.removeControl(controlMap);
				removeControlMap(controlOptions.key);
			}
		});
	};

	var hideRoutingControls = function () {
		$.each(controlsOptions, function (index, controlOptions) {
			controlOptions.visible = false;
		});
		$.each(controlsMap, function (index, controlMap) {
			map.removeControl(controlMap);
		});
		controlsMap = [];
	};

    var panMap = function (lat, lng) {
        map.panTo(new L.LatLng(lat, lng));
    };

    var centerMap = function (e) {
        panMap(e.latlng.lat, e.latlng.lng);
    };

	var setAutomaticView = function (zoom = 5) {
		let myPoints = [];
		for (let iPos=0; iPos<controlsMap.length; iPos++) {
			for (let jPos=0; jPos<controlsMap[iPos].options.waypoints.length - 1; jPos++) {
				let latLng = controlsMap[iPos].options.waypoints[jPos].latLng;
				myPoints.push(latLng);
			}
		}
		let newBounds = new L.LatLngBounds(myPoints);
		mapBounds = newBounds;
		map.fitBounds(newBounds);
	};

	var setView = function (latLng, zoom) {
		map.setView(latLng, zoom);
	};

	var resetView = function () {
		map.setView(settings.center, settings.minZoom);
	};
	
    var initMap = function () {
        let $this = this;
		let corner1 = L.latLng(settings.cornerSouth[0], settings.cornerSouth[1]);
		let corner2 = L.latLng(settings.cornerNorth[0], settings.cornerNorth[1]);
		let bounds = L.latLngBounds(corner1, corner2);

		L.Icon.Default.imagePath = 'images/';
		
		map = L.map(mapId, { 
			center: settings.center,
			maxBounds: bounds,
            attributionControl: true,
            crs: L.CRS.EPSG3857,
			zoom: settings.zoom,
			zoomSnap: settings.zoomSnap,
			zoomDelta: settings.zoomDelta,
			minZoom: settings.minZoom,
			maxZoom: settings.maxZoom,
			bounceAtZoomLimits: settings.bounceAtZoomLimits
		});

		mapboxGL = L.mapboxGL({
			attribution: '<a href="https://www.maptiler.com/license/maps/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank"> | &copy; OpenStreetMap</a>',
			accessToken: 'not-needed',
			style: 'https://maps.tilehosting.com/c/3b33897a-ffc9-4f36-8b7b-f555bd6481d9/styles/mapa-0001/style.json?key=dLbzt96PKK6rA56B4jaU'
		}).addTo(map);

		// Basic -> https://maps.tilehosting.com/styles/basic/style.json?key=dLbzt96PKK6rA56B4jaU
		// OSM Bright -> https://openmaptiles.github.io/osm-bright-gl-style/style-cdn.json
		// Positron -> https://openmaptiles.github.io/positron-gl-style/style-cdn.json
		// Dark Matter -> https://openmaptiles.github.io/dark-matter-gl-style/style-cdn.json
		// Klokantech Basic -> https://openmaptiles.github.io/klokantech-basic-gl-style/style-cdn.json
		// Ruta 0 -> https://m.ruta0.net/styles/klokantech-basic/style.json
    };

    return {
        init: init, 
		addRoutingControl: addRoutingControl, 
		controlIsMarked: controlIsMarked,
		markRoutingControl: markRoutingControl, 
		unmarkRoutingControl: unmarkRoutingControl, 
		unmarkAllRoutingControl: unmarkAllRoutingControl, 
		showRoutingControls: showRoutingControls,
		hideRoutingControls: hideRoutingControls,
        panMap: panMap,
		setAutomaticView: setAutomaticView,
		setView: setView,
		resetView: resetView,
		getMap: getMap
    }
}(jQuery));