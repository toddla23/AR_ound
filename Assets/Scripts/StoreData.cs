[System.Serializable]
public class StoreData
{
    public string id;
    public string name;
    public string description;

    public StoreData(string id, string name, string desc = "")
    {
        this.id = id;
        this.name = name;
        this.description = desc;
    }
}
