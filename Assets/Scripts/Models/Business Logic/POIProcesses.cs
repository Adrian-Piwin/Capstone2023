using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public  class POIProcesses
{
    private DBService dbContext;
    private FirebaseService fbContext;
    private string campusContext;

    public POIProcesses(DBService context, string campusID)
    {
        dbContext = context;
        campusContext = campusID;
    }

    public POIProcesses(FirebaseService fbContext, DBService dbContext, string campusID)
    {
        this.fbContext = fbContext;
        this.dbContext = dbContext;
        campusContext = campusID;
    }

    public POI getPOI()
    {
        POI found = DataTableMapper.MapDataRowToObject<POI>(dbContext.executeQuery($"select top 1 * from POI where campusID = {campusContext} order by [order] desc"));
        return found;
    }

    public async Task<byte[]> getImage(string id, string type, string fileName)
    {
        return await fbContext.GetImageBytes($"{campusContext}/{id}/{type}/{fileName}");
    }
}

