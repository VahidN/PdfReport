using System;
using System.IO;
using Windows.Storage.Streams;

namespace Win81PDFViewer
{
    public static class MicrosoftStreamExtensions
    {
        public static IRandomAccessStream AsRandomAccessStream(this Stream stream)
        {
            return new RandomStream(stream);
        }

    }

    public class RandomStream : IRandomAccessStream
    {
        readonly Stream _internstream;

        public RandomStream(Stream underlyingstream)
        {
            _internstream = underlyingstream;
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            _internstream.Position = (long)position;
            return _internstream.AsInputStream();
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            _internstream.Position = (long)position;
            return _internstream.AsOutputStream();
        }

        public ulong Size
        {
            get
            {
                return (ulong)_internstream.Length;
            }
            set
            {
                _internstream.SetLength((long)value);
            }
        }

        public bool CanRead
        {
            get { return _internstream.CanRead; }
        }

        public bool CanWrite
        {
            get { return _internstream.CanWrite; }
        }

        public IRandomAccessStream CloneStream()
        {
            throw new NotSupportedException();
        }

        public ulong Position
        {
            get { return (ulong)_internstream.Position; }
        }

        public void Seek(ulong position)
        {
            _internstream.Seek((long)position, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            _internstream.Dispose();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return GetInputStreamAt(Position).ReadAsync(buffer, count, options);
        }

        public Windows.Foundation.IAsyncOperation<bool> FlushAsync()
        {
            return GetOutputStreamAt(Position).FlushAsync();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return GetOutputStreamAt(Position).WriteAsync(buffer);
        }
    }
}