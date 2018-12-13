using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

namespace RuntimeArtWay
{
    public interface IBagOfRequests
    {
        int Count { get; }
        IRequestForShape RandomRequest { get; }
    }

    public class BagOfRequests : ScriptableObject, IBagOfRequests
    {
        [SerializeField] private List<RequestForShape> requests = new List<RequestForShape>();

        public int Count => requests.Count;

        public IRequestForShape RandomRequest => requests[0];
    }
}