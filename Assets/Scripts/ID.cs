using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ID {

    public const int ID_COUNT = 256;

    public static List<ID> GetNewIDQueue () {
        var output = new List<ID>();
        for(int i=0; i<ID.ID_COUNT; i++){
            output.Add(new ID(i));
        }
        return output;
    }

    public static ID GetNext () {
        return GetNext(GameState.current.idQueue);
    }

    public static ID GetNext (IList<ID> ids) {
        var output = ids[0];
        ids.RemoveAt(0);
        return output;
    }

    public static void ReturnId (ID id) {
        GameState.current.idQueue.Add(id);
    }

    [SerializeField] private int m_id;

    private ID (int id) {
        this.m_id = id;
    }

    public override string ToString () {
        return m_id.ToString("x2");;
    }

    public override bool Equals (object obj) {
        if(obj is ID otherId){
            return otherId.m_id == this.m_id;
        }
        if(obj is int otherInt){
            return otherInt == this.m_id;
        }
        if(obj is string otherString){
            try{
                var parsedOtherInt = int.Parse(otherString, System.Globalization.NumberStyles.HexNumber);
                return parsedOtherInt == this.m_id;
            }catch{
                return false;
            }
        }
        return false;
    }

    public override int GetHashCode () => m_id;

}
