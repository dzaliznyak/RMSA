using Newtonsoft.Json;
using Rmsa.Core.DataSource;
using Rmsa.Core.DataSource.DataParser;
using Rmsa.Model;
using System;
using System.IO;

namespace Rmsa.Services
{
    public interface IDataSourceService
    {
        event EventHandler<IDataSource> DataSourceChanged;
        DataSourceSettings Settings { get; }

        void ApplyNewSettings(DataSourceSettings settings);
        void Init();
    }

    public class DataSourceService : IDataSourceService
    {
        DataSourceSettings _dataSourceSettings;
        IDataSource _dataSource;

        public event EventHandler<IDataSource> DataSourceChanged;
        public DataSourceSettings Settings => _dataSourceSettings;

        public DataSourceService()
        {
        }

        public void Init()
        {
            _dataSourceSettings = new DataSourceSettings();
            if (File.Exists(Defines.DataSourceConfigFileName))
            {
                var serialized = File.ReadAllText(Defines.DataSourceConfigFileName);
                _dataSourceSettings = JsonConvert.DeserializeObject<DataSourceSettings>(serialized);
            }
            CreateDataSource(_dataSourceSettings);
        }

        public void ApplyNewSettings(DataSourceSettings settings)
        {
            CreateDataSource(settings);

            var serialized = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(Defines.DataSourceConfigFileName, serialized);
        }

        void CreateDataSource(DataSourceSettings settings)
        {
            if (_dataSource != null)
            {
                _dataSource.Disconnect();
                _dataSource = null;
            }

            _dataSourceSettings = settings;

            IDataParser parser = null;
            switch (settings.DataFormat)
            {
                case DataFormat.Esper:
                    parser = new EsperDataParser(settings);
                    break;
                case DataFormat.Srul:
                    parser = new SrulDataParser(settings);
                    break;
                case DataFormat.MagDrone:
                    parser = new MagDroneDataParser(settings);
                    break;
                default:
                    break;
            }

            switch (settings.DataSourceType)
            {
                case DataSourceType.File:
                    _dataSource = new FileDataSource(settings, parser);
                    break;
                case DataSourceType.ComPort:
                    _dataSource = new SerialTextDataSource(settings, parser);
                    break;
                case DataSourceType.Generator:
                    _dataSource = new GeneratorDataSource(settings);
                    break;
                default:
                    break;
            }

            DataSourceChanged?.Invoke(this, _dataSource);

            _dataSource.Connect();
        }

    }
}
