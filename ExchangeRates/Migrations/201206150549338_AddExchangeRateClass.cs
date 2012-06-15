namespace ExchangeRates.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddExchangeRateClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ExchangeRates",
                c => new
                    {
                        Date = c.DateTime(nullable: false),
                        Currency = c.String(nullable: false, maxLength: 128),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 6),
                    })
                .PrimaryKey(t => new { t.Date, t.Currency });
            
        }
        
        public override void Down()
        {
            DropTable("ExchangeRates");
        }
    }
}
