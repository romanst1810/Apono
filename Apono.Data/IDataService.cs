using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apono.Data.Models;

namespace Apono.Data
{
    public interface IDataService
    {
        Task<DatasetDictionary> GetDataSetAsync(string datasetFile, CancellationToken ct);
        Task<bool> CanVisitAsync(string citizen, string place, DatasetDictionary datasetDictionary, CancellationToken ct);
    }
}
