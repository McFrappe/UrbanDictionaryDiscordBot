public class DefinitionObject
{
    public string definition { get; set; }
    public string permalink { get; set; }
    public int thumbs_up { get; set; }
    public string author { get; set; }
    public string word { get; set; }
    public int defid { get; set; }
    public string current_vote { get; set; }
    public DateTime written_on { get; set; }
    public string example { get; set; }
    public int thumbs_down { get; set; }
}

public class DefinitionsList
{
    public List<DefinitionObject> list { get; set; }
}