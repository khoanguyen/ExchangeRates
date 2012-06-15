namespace ExchangeRates.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddDequeueMessageStoredProcedure : DbMigration
    {
        /// <summary>
        /// Add DequeuMessage Stored Procedure for dequeue a RequestMessage from the database
        /// </summary>
        public override void Up()
        {
            Sql(@"
            CREATE PROC DequeueMessage @UniqueToken AS NCHAR(30)
            AS
            BEGIN TRANSACTION Dequeue
            UPDATE RequestMessages SET Token=@UniqueToken WHERE Token IS NULL AND MessageID = (SELECT MIN(MessageID) FROM RequestMessages WHERE Token IS NULL)
            SELECT * FROM RequestMessages WHERE Token=@UniqueToken
            COMMIT TRANSACTION Dequeue");
        }
        
        public override void Down()
        {
            Sql(@"DROP PROC DeQueueMessage");
        }
    }
}
