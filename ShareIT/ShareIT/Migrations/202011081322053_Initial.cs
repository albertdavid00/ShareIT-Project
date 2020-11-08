namespace ShareIT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Content = c.String(nullable: false),
                        Date = c.DateTime(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        Profile_ProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.Profile_ProfileId)
                .Index(t => t.Profile_ProfileId);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ProfileId = c.Int(nullable: false, identity: true),
                        ProfileName = c.String(nullable: false),
                        ProfileDescription = c.String(nullable: false),
                        SignUpDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "Profile_ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.Comments", "PostId", "dbo.Posts");
            DropIndex("dbo.Posts", new[] { "Profile_ProfileId" });
            DropIndex("dbo.Comments", new[] { "PostId" });
            DropTable("dbo.Profiles");
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
        }
    }
}
