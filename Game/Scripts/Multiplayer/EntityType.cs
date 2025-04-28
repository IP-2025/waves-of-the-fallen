public class EntityType
{
    public static EntityType Player = new EntityType("Player");
    public static EntityType Enemy = new EntityType("Enemy");

    private string _name;
    
    private EntityType(string name) 
    { 
        _name = name; 
    }

    public override string ToString() 
    { 
        return _name; 
    }
}