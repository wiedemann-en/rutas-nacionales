using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Utils.Extensions
{
    public static class ExceptionExtensionMethods
    {
        private static readonly string _ErrorFormat = "Source: {0}/LineNumber: {1}/Column: {2}/File: {3}";

        public static string GetDescription(this Exception ex)
        {
            var messages = new Queue<string>();
            var exception = ex;

            do
            {
                if (ex.Message != null)
                    messages.Enqueue(exception.Message);
                exception = exception.InnerException;
            }
            while (exception != null);

            if (messages.Any())
            {
                return messages.Aggregate
                (
                    new StringBuilder(),
                    (acc, itm) => acc.Append(itm).Append(" | "),
                    acc => acc.ToString().TrimEnd(new char[] { ' ', '|' })
                );
            }

            return string.Empty;
        }

        public static string GetStackTrace(this Exception ex)
        {
            var interrupted = false;
            var tracko = new StackTrace(ex, true);
            List<StackFrame> frames = null;
            if (tracko.FrameCount > 4)
            {
                frames = new List<StackFrame>(tracko.GetFrames().Take(2));
                frames.AddRange(tracko.GetFrames().Skip(tracko.FrameCount - 3));
                interrupted = true;
            }
            else
            {
                frames = tracko.GetFrames().ToList();
            }
            if (frames != null && frames.Any())
            {
                return frames.Aggregate
                (
                    new Queue<string>(),
                    (acc, itm) =>
                    {
                        acc.Enqueue
                        (
                            string.Format(_ErrorFormat,
                                itm.GetMethod().Name,
                                itm.GetFileLineNumber(),
                                itm.GetFileColumnNumber(),
                                itm.GetFileName().GetResumePath()) + ". "
                        );
                        if (interrupted && acc.Count == 2)
                            acc.Enqueue(@" /.../ ");
                        return acc;
                    },
                    acc => acc.Aggregate(new StringBuilder(), (strbld, str) => strbld.Append(str).AppendLine(), strbld => strbld.ToString())
                );
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetDetail(this Exception ex)
        {
            var frames = new Queue<StackFrame>();
            var exception = ex;

            do
            {
                var tracko = new StackTrace(ex, true);
                if (tracko != null && tracko.FrameCount > 0)
                    frames.Enqueue(tracko.GetFrame(tracko.FrameCount - 1));
                exception = exception.InnerException;
            }
            while (exception != null);

            if (frames.Any())
            {
                return frames.Aggregate
                (
                    new Queue<string>(),
                    (acc, itm) =>
                    {
                        acc.Enqueue
                        (
                            string.Format(_ErrorFormat,
                                itm.GetMethod().Name,
                                itm.GetFileLineNumber(),
                                itm.GetFileColumnNumber(),
                                itm.GetFileName().GetResumePath()) + ". "
                        );
                        return acc;
                    },
                    acc => acc.Aggregate(new StringBuilder(), (strbld, str) => strbld.Append(str).AppendLine(), strbld => strbld.ToString())
                );
            }

            return string.Empty;
        }
    }
}
