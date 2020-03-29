using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vialidad.Contracts.Enums;
using Vialidad.Download;

namespace Vialidad.Fixture
{
    public class ScraperFixture
    {
        private List<DownloadBase> _fixture;

        public ScraperFixture()
        {
            InitializeFixture();
        }

        public void ExecuteFixture()
        {
            foreach (var itemFixture in _fixture)
                itemFixture.Download();
        }

        private void InitializeFixture()
        {
            _fixture = new List<DownloadBase>();
            //_fixture.Add(new DownloadExcel(enDataProviderType.Vialidad));
            //_fixture.Add(new DownloadExcel(enDataProviderType.Google));
            //_fixture.Add(new DownloadDb(enDataProviderType.Vialidad));
            _fixture.Add(new DownloadDb(enDataProviderType.Google));
        }
    }
}
