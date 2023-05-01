namespace StravaProfisee.StravaModels;

public class Activity {
    public string name {get; set; }

    public double elapsed_time {get; set;}

    public MetaAthlete athlete {get; set;}
}

public class MetaAthlete {
    public string firstname {get; set;}
    public string lastname {get; set;}
}
