using UnityEngine;

// A blank base class that all future traits will inherit from

public abstract class BlockProperty : ScriptableObject 
{
    // Can leave this empty as just a minimal tag 
    // or add functions / properties to inform behaviors
}

[CreateAssetMenu(fileName = "IceBlockProperty", menuName = "Blocks/BlockProperties/IceBlockProperty")]
public class IceBlockProperty : BlockProperty
{
    
}