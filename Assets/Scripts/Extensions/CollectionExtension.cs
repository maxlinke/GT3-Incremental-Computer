using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtension {

    public static int IndexOf<T> (this IReadOnlyList<T> collection, T element) {
        for(int i=0; i<collection.Count; i++){
            if(ReferenceEquals(collection[i], null)){
                if(ReferenceEquals(element, null)){
                    return i;
                }
            }else if(collection[i].Equals(element)){
                return i;
            }
        }
        return -1;
    }

}
