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

    public POI getPOI(string index)
    {
        POI found = DataTableMapper.MapDataRowToObject<POI>(dbContext.executeQuery($"select top 1 * from POI where campusID = {campusContext} and [order] = {index}"));
        return found;
    }

    public async Task<byte[]> getImage(string id, string type, string fileName)
    {
        return await fbContext.GetImageBytes($"{campusContext}/{id}/{type}/{fileName}");
    }

    public List<POI> getAllPOI() 
    {
        List<POI> pois = DataTableMapper.MapDataTableToObjectList<POI>(dbContext.executeQuery($"select * from POI where campusID = {campusContext}"));
        return pois;
    }
}

