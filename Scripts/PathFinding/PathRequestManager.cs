using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void PathResult(HexTile[] path, bool result);

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
    private PathRequest _currentPathRequest;
    private bool _isProcessing;
    
    private static PathRequestManager _instance;    
    public static PathRequestManager Instance { get => _instance; set => _instance = value; }



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("[PathRequestManager]: Someone is trying to initialize pathfinder");
        }
    }


    public void RequestPath(Vector3 pathStart, Vector3 pathEnd, PathResult pathResultCallback)
    {
        PathRequest newPathRequest = new PathRequest(pathStart, pathEnd, pathResultCallback);
        Instance._pathRequestQueue.Enqueue(newPathRequest);
        Instance.TryProcesNext();

    }

    private void TryProcesNext()
    {
        //TODO: asynchronnous?
        if (!_isProcessing && _pathRequestQueue.Count > 0)
        {
            _currentPathRequest = _pathRequestQueue.Dequeue();
            _isProcessing = true;
            PathFinder.Instance.StartFindPath(_currentPathRequest._pathStart, _currentPathRequest._pathEnd, PathProcessingFinished);
        }

    }

    private void PathProcessingFinished(HexTile[] path, bool success)
    {
        _currentPathRequest._pathResultCallback(path, success);
        _isProcessing = false;
        TryProcesNext();
    }

    struct PathRequest
    {
        public Vector3 _pathStart;
        public Vector3 _pathEnd;
        public PathResult _pathResultCallback;

        public PathRequest(Vector3 pathStart, Vector3 pathEnd, PathResult pathResultCallback)
        {
            _pathStart = pathStart;
            _pathEnd = pathEnd;
            _pathResultCallback = pathResultCallback;
        }
    }
}
