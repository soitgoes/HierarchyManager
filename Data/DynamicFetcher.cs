using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DynamicFetcher 
    {
        private readonly IDbConnection connection;
        private readonly string tableName;
        private string idField;
        private readonly string parentIdField;
        private string nameField;

        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"> Connection should be managed by the application</param>
        public DynamicFetcher(IDbConnection connection, string tableName, string idField, string parentId, string name)
        {
            this.connection = connection;
            this.tableName = tableName;
            this.idField = idField;
            this.parentIdField = parentId;
            this.nameField = name;
        }

        public IEnumerable<NamedItem> GetRoot(int skip=0, int take=50)
        {
            return connection.Query<NamedItem>($"select {idField} Id, {nameField} Name from {tableName} where {parentIdField} is null order by {nameField} OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", new {skip, take});
        }
        public IEnumerable<NamedItem> GetChildren(int parentFilterId, int skip=0, int take = 50)
        {
            return connection.Query<NamedItem>($"select {idField} Id, {nameField} Name from {tableName} where {parentIdField} = @parentFilterId order by {nameField} OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", new { parentFilterId, skip, take });
        }
        public void MoveItem(int id, int destinationParentId)
        {
            connection.Execute($"update {tableName} set {parentIdField}=@parentId where {idField} = @id", new { id, parentId = destinationParentId });
        }

        public IEnumerable<NamedItem> Search(string term, int skip=0, int take = 50)
        {
            term = $"%{term}%";
            return connection.Query<NamedItem>($"select {idField} Id, {nameField} Name from {tableName} where {nameField} like @term order by {nameField} OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY", new { term, skip, take });
        }
        

    }
}
