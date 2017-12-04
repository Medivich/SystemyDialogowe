using System.IO;


namespace Dialogowe.BazaDanych
{
    class Polaczenie
    {

        //Nie wiem na huj jest \Debug -.- 
        //Wywołuje mi program niby z folderu nad plikiem .exe 
        //Jebany framework 4.0

        public static string connString = @"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename="
                    + Path.GetDirectoryName(System.Environment.CurrentDirectory) +
                    @"\Debug\BazaDanych\Database.mdf;Integrated Security=True";

        public string ConnString
        {
            get { return connString; }
            set
            {
                connString = value;
            }
        }
    }
}
