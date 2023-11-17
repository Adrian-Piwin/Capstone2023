using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using static ZXing.QrCode.Internal.Mode;

public  class PlayerProcesses
{
    private DBService dbContext;
    private string codeContext;

    public PlayerProcesses(DBService context, string code)
    {
        dbContext = context;
        codeContext = code;
    }

    public Player createPlayer(string name)
    {
        dbContext.executeNonQuery($"insert into Players (lobbyID, username, status) values ('{codeContext}', '{name}', '1')");

        Player player = DataTableMapper.MapDataRowToObject<Player>(dbContext.executeQuery($"select top 1 * from players where username = '{name}' order by id desc"));
        return player;
    }

    public Player getPlayer(string id) 
    {
        Player player = DataTableMapper.MapDataRowToObject<Player>(dbContext.executeQuery($"select top 1 * from players where id = {id} and lobbyID = {codeContext}"));
        return player;
    }

    public void updatePlayerStatus(string id, string status) 
    {
        dbContext.executeNonQuery($"update players set status = {status} where id = {id}");
    }

    public List<Player> getPlayers()
    {
        return DataTableMapper.MapDataTableToObjectList<Player>(dbContext.executeQuery($"select * from Players where lobbyID = {codeContext}"));
    }
}

