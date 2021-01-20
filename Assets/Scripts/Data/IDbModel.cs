using System.Data;
using System.Data.Common;

namespace CoronaGame.Data
{
    /// <summary>
    /// Object that can work as model in this custom
    /// system.
    /// </summary>
    public interface IDbModel
    {
        int Id { get; }
        void CreateFromRow(IDataReader reader);
        void SetInsertParameters(DbCommand dbCommand);
        void SetUpdateParameters(DbCommand dbCommand);
    }
}

