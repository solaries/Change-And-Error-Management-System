using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;

namespace sphinxsolaries.Caems.Data.Models 
{
	public partial class Caems : Database
	{
		public Caems() : base("Caems")
		{
			CommonConstruct();
		}
		public virtual void CommonConstruct()
		{
		    Factory = new DefaultFactory();
		}
		public interface IFactory
		{
			Caems GetInstance();
		    void BeginTransaction(Caems database);
		    void CompleteTransaction(Caems database);
		}


        public class DefaultFactory : IFactory
        {
            [ThreadStatic]
            static Stack<Caems> _stack = new Stack<Caems>();

            public Caems GetInstance()
            {
               
			    if (_stack == null)
                { return new  Caems(); }
                else { 
					return _stack.Count > 0 ? _stack.Peek() : new Caems();
                }
			   
			    
            }

            public void BeginTransaction(Caems database)
            {

			 if (_stack == null)
				 {
				  _stack = new  Stack<Caems>();
				 }
                _stack.Push(database);
            }

            public void CompleteTransaction(Caems database)
            {
			 if (_stack == null)
				 {
				  _stack = new Stack <Caems>();
				 }
                _stack.Pop();
            }
        }
		
		public static IFactory Factory { get; set; }

        public static Caems GetInstance()
        {
		 if (Factory == null)
                return new Caems();
			return Factory.GetInstance();
        }

		protected override void OnBeginTransaction()
		{
            Factory.BeginTransaction(this);
		}

        protected override void OnCompleteTransaction()
		{
            Factory.CompleteTransaction(this);
		}

		public class Record<T> where T:new()
		{
			public bool IsNew(Database db) { return db.IsNew(this); }
			public object Insert(Database db) { return db.Insert(this); }  
			
			public int Update(Database db, IEnumerable<string> columns) { return db.Update(this, columns); }
			public static int Update(Database db, string sql, params object[] args) { return db.Update<T>(sql, args); }
			public static int Update(Database db, Sql sql) { return db.Update<T>(sql); }
			public int Delete(Database db) { return db.Delete(this); }
			public static int Delete(Database db, string sql, params object[] args) { return db.Delete<T>(sql, args); }
			public static int Delete(Database db, Sql sql) { return db.Delete<T>(sql); }
			public static int Delete(Database db, object primaryKey) { return db.Delete<T>(primaryKey); }
			public static bool Exists(Database db, object primaryKey) { return db.Exists<T>(primaryKey); }
			public static T SingleOrDefault(Database db, string sql, params object[] args) { return db.SingleOrDefault<T>(sql, args); }
			public static T SingleOrDefault(Database db, Sql sql) { return db.SingleOrDefault<T>(sql); }
			public static T FirstOrDefault(Database db, string sql, params object[] args) { return db.FirstOrDefault<T>(sql, args); }
			public static T FirstOrDefault(Database db, Sql sql) { return db.FirstOrDefault<T>(sql); }
			public static T Single(Database db, string sql, params object[] args) { return db.Single<T>(sql, args); }
			public static T Single(Database db, Sql sql) { return db.Single<T>(sql); }
			public static T First(Database db, string sql, params object[] args) { return db.First<T>(sql, args); }
			public static T First(Database db, Sql sql) { return db.First<T>(sql); }
			public static List<T> Fetch(Database db, string sql, params object[] args) { return db.Fetch<T>(sql, args); }
			public static List<T> Fetch(Database db, Sql sql) { return db.Fetch<T>(sql); }
			public static List<T> Fetch(Database db, long page, long itemsPerPage, string sql, params object[] args) { return db.Fetch<T>(page, itemsPerPage, sql, args); }
			public static List<T> Fetch(Database db, long page, long itemsPerPage, Sql sql) { return db.Fetch<T>(page, itemsPerPage, sql); }
			public static List<T> SkipTake(Database db, long skip, long take, string sql, params object[] args) { return db.SkipTake<T>(skip, take, sql, args); }
			public static List<T> SkipTake(Database db, long skip, long take, Sql sql) { return db.SkipTake<T>(skip, take, sql); }
			public static Page<T> Page(Database db, long page, long itemsPerPage, string sql, params object[] args) { return db.Page<T>(page, itemsPerPage, sql, args); }
			public static Page<T> Page(Database db, long page, long itemsPerPage, Sql sql) { return db.Page<T>(page, itemsPerPage, sql); }
			public static IEnumerable<T> Query(Database db, string sql, params object[] args) { return db.Query<T>(sql, args); }
			public static IEnumerable<T> Query(Database db, Sql sql) { return db.Query<T>(sql); }			
			
			protected HashSet<string> Tracker = new HashSet<string>();
			private void OnLoaded() { Tracker.Clear(); }
			protected void Track(string c) { if (!Tracker.Contains(c)) Tracker.Add(c); }

			public int Update(Database db) 
			{ 
				if (Tracker.Count == 0)
					return db.Update(this); 

				var retv = db.Update(this, Tracker.ToArray());
				Tracker.Clear();
				return retv;
			}
			public void Save(Database db) 
			{
                if (this.IsNew(db))
					Insert(db);
				else
					Update(db);
			}		
		}	
	}
	 [TableName("CAEMS_event")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class @Event : Caems.Record<@Event>

		{
			[Column("id")] public long Id 
			{ 
				get { return _Id; }
				set { _Id = value; Track("id"); }
			}
			long _Id;
			[Column("eventName")] public string Eventname 
			{ 
				get { return _Eventname; }
				set { _Eventname = value; Track("eventName"); }
			}
			string _Eventname;
		
			public static IEnumerable<@Event> Query(Database db, string[] columns = null, long[] Id = null)
            {
                var sql = new Sql();

                if (columns != null)
                    sql.Select(columns);

                sql.From("CAEMS_event (NOLOCK)");


				if (Id != null)
					sql.Where("id IN (@0)", Id);


                return db.Query<@Event>(sql);
            }

		} [TableName("CAEMS_eventLog")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class EventLog : Caems.Record<EventLog>

		{
			[Column("id")] public long Id 
			{ 
				get { return _Id; }
				set { _Id = value; Track("id"); }
			}
			long _Id;
			[Column("eventid")] public long Eventid 
			{ 
				get { return _Eventid; }
				set { _Eventid = value; Track("eventid"); }
			}
			long _Eventid;
			[Column("description")] public string Description 
			{ 
				get { return _Description; }
				set { _Description = value; Track("description"); }
			}
			string _Description;
			[Column("userEvent")] public bool Userevent 
			{ 
				get { return _Userevent; }
				set { _Userevent = value; Track("userEvent"); }
			}
			bool _Userevent;
			[Column("userid")] public long Userid 
			{ 
				get { return _Userid; }
				set { _Userid = value; Track("userid"); }
			}
			long _Userid;
			[Column("eventDate")] public DateTime Eventdate 
			{ 
				get { return _Eventdate; }
				set { _Eventdate = value; Track("eventDate"); }
			}
			DateTime _Eventdate;
		
			public static IEnumerable<EventLog> Query(Database db, string[] columns = null, long[] Id = null)
            {
                var sql = new Sql();

                if (columns != null)
                    sql.Select(columns);

                sql.From("CAEMS_eventLog (NOLOCK)");


				if (Id != null)
					sql.Where("id IN (@0)", Id);


                return db.Query<EventLog>(sql);
            }

		} [TableName("CAEMS_authenticate_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_authenticate_Admin : Caems.Record<CAEMS_authenticate_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
     [Column("service_company")]  
     public  long  Service_company  
     {  
         get { return _Service_company; }  
         set { _Service_company = value; Track("service_company"); }  
      }  
      long  _Service_company;  
    
     [Column("leadadmin")]  
     public   Int16   Leadadmin  
     {  
         get { return _Leadadmin; }  
         set { _Leadadmin = value; Track("leadadmin"); }  
      }  
       Int16   _Leadadmin;  
    
 public static IEnumerable<CAEMS_authenticate_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_authenticate_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_authenticate_Admin>(sql);
     }
  }


 [TableName("CAEMS_authenticate_SuperAdmin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_authenticate_SuperAdmin : Caems.Record<CAEMS_authenticate_SuperAdmin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
 public static IEnumerable<CAEMS_authenticate_SuperAdmin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_authenticate_SuperAdmin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_authenticate_SuperAdmin>(sql);
     }
  }


 [TableName("CAEMS_authenticate_User")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_authenticate_User : Caems.Record<CAEMS_authenticate_User>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
     [Column("organization")]  
     public  long  Organization  
     {  
         get { return _Organization; }  
         set { _Organization = value; Track("organization"); }  
      }  
      long  _Organization;  
    
     [Column("lead_user")]  
     public   Int16   Lead_user  
     {  
         get { return _Lead_user; }  
         set { _Lead_user = value; Track("lead_user"); }  
      }  
       Int16   _Lead_user;  
    
     [Column("service_company")]  
     public  long  Service_company  
     {  
         get { return _Service_company; }  
         set { _Service_company = value; Track("service_company"); }  
      }  
      long  _Service_company;  
    
 public static IEnumerable<CAEMS_authenticate_User> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_authenticate_User (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_authenticate_User>(sql);
     }
  }


 [TableName("CAEMS_Change_Or_Error")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_Change_Or_Error : Caems.Record<CAEMS_Change_Or_Error>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("project")]  
     public  long  Project  
     {  
         get { return _Project; }  
         set { _Project = value; Track("project"); }  
      }  
      long  _Project;

      [Column("Initiating_User_Type")]
      public long Initiating_User_Type
      {
          get { return _Initiating_User_Type; }
          set { _Initiating_User_Type = value; Track("Initiating_User_Type"); }
      }
      long _Initiating_User_Type;  

      [Column("User")]
      public long User
      {
          get { return _User; }
          set { _User = value; Track("User"); }
      }
      long _User;  
    
     [Column("change_or_error_detail")]  
     public  string   Change_or_error_detail  
     {  
         get { return _Change_or_error_detail; }  
         set { _Change_or_error_detail = value; Track("change_or_error_detail"); }  
      }  
      string   _Change_or_error_detail;  
    
     [Column("log_date")]  
     public  DateTime  Log_date  
     {  
         get { return _Log_date; }  
         set { _Log_date = value; Track("log_date"); }  
      }  
      DateTime  _Log_date;  
    
 public static IEnumerable<CAEMS_Change_Or_Error> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_Change_Or_Error (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_Change_Or_Error>(sql);
     }
  }


 [TableName("CAEMS_Change_Or_Error_Movement")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_Change_Or_Error_Movement : Caems.Record<CAEMS_Change_Or_Error_Movement>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("change")]  
     public  long  Change  
     {  
         get { return _Change; }  
         set { _Change = value; Track("change"); }  
      }  
      long  _Change;  
    
     [Column("action")]  
     public   Int16   Action  
     {  
         get { return _Action; }  
         set { _Action = value; Track("action"); }  
      }  
       Int16   _Action;  
    
     [Column("action_user_type")]  
     public   Int16   Action_user_type  
     {  
         get { return _Action_user_type; }  
         set { _Action_user_type = value; Track("action_user_type"); }  
      }  
       Int16   _Action_user_type;  
    
     [Column("action_user")]  
     public  long  Action_user  
     {  
         get { return _Action_user; }  
         set { _Action_user = value; Track("action_user"); }  
      }
     long _Action_user;

     [Column("Move_Date")]
     public DateTime Move_Date
     {
         get { return _Move_Date; }
         set { _Move_Date = value; Track("Move_Date"); }
     }
     DateTime _Move_Date; 
    
 public static IEnumerable<CAEMS_Change_Or_Error_Movement> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_Change_Or_Error_Movement (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_Change_Or_Error_Movement>(sql);
     }
  }


 [TableName("CAEMS_Client")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_Client : Caems.Record<CAEMS_Client>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("name")]  
     public  string   Name  
     {  
         get { return _Name; }  
         set { _Name = value; Track("name"); }  
      }  
      string   _Name;  
    
     [Column("service_company")]  
     public  long  Service_company  
     {  
         get { return _Service_company; }  
         set { _Service_company = value; Track("service_company"); }  
      }  
      long  _Service_company;  
    
 public static IEnumerable<CAEMS_Client> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_Client (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_Client>(sql);
     }
  }


 [TableName("CAEMS_Project")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_Project : Caems.Record<CAEMS_Project>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("project_title")]  
     public  string   Project_title  
     {  
         get { return _Project_title; }  
         set { _Project_title = value; Track("project_title"); }  
      }  
      string   _Project_title;

     [Column("Organization")]
      public long Organization  
     {
         get { return _Organization; }
         set { _Organization = value; Track("Organization"); }  
      }
     long _Organization;  
    
 
    
 public static IEnumerable<CAEMS_Project> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_Project (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_Project>(sql);
     }
  }


 [TableName("CAEMS_right_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_right_Admin : Caems.Record<CAEMS_right_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rightname")]  
     public  string   Rightname  
     {  
         get { return _Rightname; }  
         set { _Rightname = value; Track("rightname"); }  
      }  
      string   _Rightname;  
    
 public static IEnumerable<CAEMS_right_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_right_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_right_Admin>(sql);
     }
  }


 [TableName("CAEMS_right_User")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_right_User : Caems.Record<CAEMS_right_User>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rightname")]  
     public  string   Rightname  
     {  
         get { return _Rightname; }  
         set { _Rightname = value; Track("rightname"); }  
      }  
      string   _Rightname;  
    
 public static IEnumerable<CAEMS_right_User> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_right_User (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_right_User>(sql);
     }
  }


 [TableName("CAEMS_role_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_role_Admin : Caems.Record<CAEMS_role_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;


     [Column("service_company")]
     public long service_company  
     {
         get { return _service_company; }
         set { _service_company = value; Track("service_company"); }  
      }
     long _service_company;  
    

     [Column("rolename")]  
     public  string   Rolename  
     {  
         get { return _Rolename; }  
         set { _Rolename = value; Track("rolename"); }  
      }  
      string   _Rolename;  
    
 public static IEnumerable<CAEMS_role_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_role_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_role_Admin>(sql);
     }
  }


 [TableName("CAEMS_role_right_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_role_right_Admin : Caems.Record<CAEMS_role_right_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("right")]  
     public  long  Right  
     {  
         get { return _Right; }  
         set { _Right = value; Track("right"); }  
      }  
      long  _Right;  
    
 public static IEnumerable<CAEMS_role_right_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_role_right_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_role_right_Admin>(sql);
     }
  }


 [TableName("CAEMS_role_right_User")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_role_right_User : Caems.Record<CAEMS_role_right_User>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("right")]  
     public  long  Right  
     {  
         get { return _Right; }  
         set { _Right = value; Track("right"); }  
      }  
      long  _Right;  
    
 public static IEnumerable<CAEMS_role_right_User> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_role_right_User (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_role_right_User>(sql);
     }
  }


 [TableName("CAEMS_role_User")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_role_User : Caems.Record<CAEMS_role_User>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  

    
     [Column("client")]
     public long client  
     {
         get { return _client; }
         set { _client = value; Track("client"); }  
      }
     long _client;  

    
     [Column("rolename")]  
     public  string   Rolename  
     {  
         get { return _Rolename; }  
         set { _Rolename = value; Track("rolename"); }  
      }  
      string   _Rolename;  
    
 public static IEnumerable<CAEMS_role_User> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_role_User (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_role_User>(sql);
     }
  }


 [TableName("CAEMS_Service_Company")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class CAEMS_Service_Company : Caems.Record<CAEMS_Service_Company>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("company")]  
     public  string   Company  
     {  
         get { return _Company; }  
         set { _Company = value; Track("company"); }  
      }  
      string   _Company;  
    
 public static IEnumerable<CAEMS_Service_Company> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("CAEMS_Service_Company (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<CAEMS_Service_Company>(sql);
     }
  }

	}
