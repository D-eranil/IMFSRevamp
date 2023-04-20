using IMFS.Web.Models.DBModel;
using IMFS.Web.Models.OPRDBModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace IMFS.DataAccess.DBContexts
{

    public partial class ORPDBContext : DbContext, IDbContext
    {
        public ORPDBContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<ORPDBContext>(null);
        }

        public virtual DbSet<mProduct> mProduct { get; set; }

        public virtual DbSet<mCustomer> mCustomer { get; set; }
        public virtual DbSet<CustomerAddresses> CustomerAddresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            //var alreadyAttached = Set<TEntity>().Local.Where(x => x.UniqueId == entity.UniqueId).FirstOrDefault();
            //if (alreadyAttached == null)
            //{
            //    //attach new entity
            //    Set<TEntity>().Attach(entity);
            //    return entity;
            //}
            //else
            //{
            //    //entity is already loaded.
            //    return alreadyAttached;
            //}
            return entity;
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public System.Collections.Generic.IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //HACK: Entity Framework Code First doesn't support doesn't support output parameters
            //That's why we have to manually create command and execute it.
            //just wait until EF Code First starts support them
            //
            //More info: http://weblogs.asp.net/dwahlin/archive/2011/09/23/using-entity-framework-code-first-with-stored-procedures-that-have-output-parameters.aspx

            bool hasOutputParameters = false;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var outputP = p as DbParameter;
                    if (outputP == null)
                        continue;

                    if (outputP.Direction == ParameterDirection.InputOutput ||
                        outputP.Direction == ParameterDirection.Output)
                        hasOutputParameters = true;
                }
            }

            var context = ((IObjectContextAdapter)(this)).ObjectContext;
            if (!hasOutputParameters)
            {
                //no output parameters
                var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();
                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);

                return result;

                //var result = context.ExecuteStoreQuery<TEntity>(commandText, parameters).ToList();
                //foreach (var entity in result)
                //    Set<TEntity>().Attach(entity);
                //return result;
            }
            else
            {
                //var connection = context.Connection;
                var connection = this.Database.Connection;
                //Don't close the connection after command execution

                //open the connection for use
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                //create a command object
                using (var cmd = connection.CreateCommand())
                {
                    //command to execute
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // move parameters to command object
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);

                    //database call
                    var reader = cmd.ExecuteReader();
                    //return reader.DataReaderToObjectList<TEntity>();
                    var result = context.Translate<TEntity>(reader).ToList();
                    for (int i = 0; i < result.Count; i++)
                        result[i] = AttachEntityToContext(result[i]);
                    //close up the reader, we're done saving results
                    reader.Close();
                    return result;
                }
            }
        }

        public System.Collections.Generic.IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }

            var result = this.Database.ExecuteSqlCommand(sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }
    }
}

