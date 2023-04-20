using IMFS.Web.Models.DBModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace IMFS.DataAccess.DBContexts
{

    public partial class IMFSDBContext : DbContext, IDbContext
    {
        public IMFSDBContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer<IMFSDBContext>(null);
        }

        public virtual DbSet<QuoteRate> QuoteRate { get; set; }
        public virtual DbSet<QuoteBreakTotalRate> QuoteBreakTotalRate { get; set; }
        public virtual DbSet<QuoteBreakPercentRate> QuoteBreakPercentRate { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Types> Types { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Funder> Funder { get; set; }
        public virtual DbSet<FunderPlan> FunderPlan { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<FinanceType> FinanceType { get; set; }
        public virtual DbSet<FinanceProductType> FinanceProductType { get; set; }

        public virtual DbSet<IMFSAPILog> IMFSAPILog { get; set; }

        public virtual DbSet<Quotes> Quotes { get; set; }

        public virtual DbSet<Config> Config { get; set; }

        public virtual DbSet<QuoteLines> QuoteLines { get; set; }
        public virtual DbSet<QuoteLinesVersion> QuoteLinesVersion { get; set; }
        public virtual DbSet<QuoteLinesFinanceOption> QuoteLinesFinanceOption { get; set; }
        public virtual DbSet<QuoteOrigins> QuoteOrigins { get; set; }
        public virtual DbSet<QuoteLineFormat> QuoteLineFormat { get; set; }
        public virtual DbSet<QuoteDuration> QuoteDuration { get; set; }

        public virtual DbSet<VSRProductType> VSRProductType { get; set; }
        public virtual DbSet<Status> Status { get; set; }

        public virtual DbSet<Emails> Emails { get; set; }
        public virtual DbSet<EmailsArchived> EmailsArchived { get; set; }
        public virtual DbSet<EmailAttachment> EmailAttachments { get; set; }
        public virtual DbSet<EmailAttachmentsTemp> EmailAttachmentsTemp { get; set; }
        public virtual DbSet<EmailXref> EmailXref { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<vw_Emails> vw_Emails { get; set; }
        public virtual DbSet<QuoteLog> QuoteLogs { get; set; }
        public virtual DbSet<EmailsHTMLBody> EmailsHTMLBody { get; set; }
        public virtual DbSet<OTC> OTC { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }

        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<ContactsTypes> ContactsTypes { get; set; }
        public virtual DbSet<ContactsXref> ContactsXref { get; set; }
        public virtual DbSet<Attachments> Attachments { get; set; }

        public virtual DbSet<ProductXref> ProductXref { get; set; }
        public virtual DbSet<CustomerAux> CustomerAux { get; set; }
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
