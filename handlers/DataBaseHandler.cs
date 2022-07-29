using System.Data.SQLite;

namespace FPB.handlers;

public class DataBaseHandler : IDisposable
{
    private SQLiteCommand _cmd;
    private readonly SQLiteConnection _con;
    public DataBaseHandler(string cs)
    {
        string[] csArr = cs.Split('/');
        if (!Directory.Exists(string.Join("/", csArr[..^1]))) Directory.CreateDirectory(string.Join("/", csArr[..^1]));
        if (!File.Exists(cs)) SQLiteConnection.CreateFile(cs);
        _con = new SQLiteConnection($"URI=file:{cs}");
        _con.Open();
        _cmd = new SQLiteCommand(_con);
    }

    public void RunSqliteNonQueryCommand(string command)
    {
        _cmd = new SQLiteCommand(command, _con);
        _cmd.ExecuteNonQuery();
    }

    public SQLiteDataReader RunSqliteQueryCommand(string command)
    {
        _cmd = new SQLiteCommand(command, _con);
        return _cmd.ExecuteReader();
    }
    

    public void Dispose()
    {
        _cmd.Dispose();
        _con.Dispose();
    }
}