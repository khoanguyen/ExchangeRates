namespace ExchangeRates.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddRequestMessageClass : DbMigration
    {
        public override void Up()
        {            
            CreateTable(
                "RequestMessages",
                c => new
                    {
                        MessageID = c.Int(nullable: false, identity: true),
                        Token = c.String(fixedLength: true, maxLength: 30),
                        DateList = c.String(isMaxLength: true),
                    })
                .PrimaryKey(t => t.MessageID)
                .Index(t => t.Token);            
        }
        
        public override void Down()
        {
            DropTable("RequestMessages");
        }
    }
}
