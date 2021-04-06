namespace Rmsa.Core.DataSource.DataParser
{
    class DataParserHelper
    {
        readonly DataFormat _dataFormat;

        public bool IsFixedFrameWidth 
        {
            get
            {
                return _dataFormat == DataFormat.Srul;
            }
        }

        public int FrameWidth
        {
            get
            {
                switch (_dataFormat)
                {
                    case DataFormat.Esper:
                        return 1024;
                    case DataFormat.Srul:
                        return 512;
                    default:
                        return 1024;
                }
            }
        }

        public DataParserHelper(DataFormat dataFormat)
        {
            _dataFormat = dataFormat;
        }


    }
}
