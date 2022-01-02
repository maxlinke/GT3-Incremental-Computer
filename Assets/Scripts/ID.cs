using System.Collections.Generic;

public class ID {

    private static Queue<ID> ids;

    static ID () {
        ids = new Queue<ID>();
        for(int i=0; i<256; i++){
            ids.Enqueue(new ID(i));
        }
    }

    public static ID GetNext () {
        return ids.Dequeue();
    }

    public static void ReturnId (ID id) {
        ids.Enqueue(id);
    }

    private readonly int m_id;

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
