namespace Rmsa.Core.DataSource.DataParser
{
    class DataParserHelper
    {
        readonly DataFormat _dataFormat;

        public bool IsFixedFrameWidth => _dataFormat == DataFormat.Srul;

        public int FrameWidth
        {
            get
            {
                return _dataFormat switch
                {
                    DataFormat.Esper => 1024,
                    DataFormat.Srul => 512,
                    _ => 1024,
                };
            }
        }

        public DataParserHelper(DataFormat dataFormat)
        {
            _dataFormat = dataFormat;
        }


    }
}
