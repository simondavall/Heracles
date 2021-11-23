/*

	Edit Map Code v2:

	Basic Principles:
		rkPoint = the points that will make the displayed polyline
		latlng, deltaTime, deltaPause, keyPoint (if promoted as a toggle line);


	map initialize options:
	{
		editMode 	: bool,		// (are we in edit mode)
		mapDiv 		: string, 	// what element the map will live in
		snapToRoads : bool, 	// snapToRoads
		points 		: array, 	// points for the activity you are editing
		mapZoom		: int, 		// level of zoom for mat
		mapCenter	: latLng, 	// centering point for map
		distanceUnits: string,	// 'mi' or 'km'
		isLive		: bool,		// is this a live activity
		editActivity : bool		// is this a edit activity (true) or new Activity (false)
	}

*/

function GhostPoint(marker,linePointIndex)
{
	this.marker = marker;
	this.linePointIndex = linePointIndex;
}

function rkPoint(latLng,deltaTime,deltaPause,keyPoint)
{
	this.latLng = latLng;
	this.deltaTime = deltaTime;
	this.deltaPause = deltaPause;
	this.keyPoint = keyPoint;
}

// helper function for javascript object size
Object.size = function(obj) {
    var size = 0, key;
    for (key in obj) {
        if (obj.hasOwnProperty(key)) size++;
    }
    return size;
};

// some utility functions
var distanceFunctions = {
	KM_PER_MILE : 1.609344,
	PI : 3.14159265358979323846,

	flooredNum : function(number,decimals)
	{
		var multiplier = Math.pow(10,decimals);
		return Math.floor((number)*multiplier)/multiplier;
	},

	rad : function(x)
	{
		return x * distanceFunctions.PI / 180;
	},

	rawDistHaversine : function(lat1, lng1, lat2, lng2)
	{
		var R = 6371; // earth's mean radius in km
		var dLat = distanceFunctions.rad(lat2 - lat1);
		var dLong = distanceFunctions.rad(lng2 - lng1);

		var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(distanceFunctions.rad(lat1)) * Math.cos(distanceFunctions.rad(lat2)) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
		var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
		var d = R * c;

		// return distance in meters
		return (d*1000);
	},

	distHaversine : function(p1, p2)
	{
		return distanceFunctions.rawDistHaversine(p1.lat, p1.lng, p2.lat, p2.lng);  
	}
}

function roundNumber(number,decimalPlaces)
{
	var multiplier = Math.pow(10, decimalPlaces);
	return Math.round(number * multiplier) / multiplier;
}

var mapController = {
      
   MAX_GHOST_POINTS : 50,
   
	markerOptions : {
		startMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/start.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,30]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity'	: 1,
			'zIndexOffset' : 899
		},
		pauseMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/pause.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,30]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity'	: 1,
			'zIndexOffset' : 897
		},
		resumeMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/resume.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,30]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity'	: 1,
			'zIndexOffset' : 898
		},
		normalMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/defaultPoint.png',
				'iconSize' 		: [14,14],
				'iconAnchor' 	: [7,7]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity'	: 1,
			'zIndexOffset' : 896
		},
		ghostMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/defaultPoint.png',
				'iconSize' 		: [10,10],
				'iconAnchor' 	: [5,5]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity'	: 0,
			'zIndexOffset' : 500
		},
		finishMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/end.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,30]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity' 	: 1,
			'zIndexOffset' : 900
		},
		viewPointMarker : {
			'icon'		: new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/realtime.png',
				'iconSize' 		: [50,50],
				'iconAnchor' 	: [25,33]
			}),
			'clickable'	: true,
			'draggable' : true,
			'opacity'	: 1,
			'zIndexOffset' : 901
		}
	},
	model : {
		// map
		map 		: null,

		// activity polyline
		activityPolylines 	: [],

		// thumbnail polylines
		thumbnailPolylines : [],

		// race polyline
		routePolyline 	: null,

		// route polyline
		racePolyline 	: null,

		// snapToRoads
		snapToRoads 	: false,

		// activity points
		linePoints 	: [],

		// all ghost points
		ghostPoints : [],

		// all distance Markers
		distanceMarkers : [],

		// undo actions
		snapShotStack	: [],

		// dragging var to prevent click event when finished
		justFinishedDragging : false,

		// thumpnailMap
		thumbnailMap : null,

		// callback functions for edit
		mapEdited : [],

		// callback functions for reset
		mapReset : [],

		// miles or km
		distanceUnits : 'mi',

		// distance for callback functions
		distance : 0,

		// marker for live trips
		liveActivityMarker : null,

		// view point for checking a specific point on map (ex. chart hover)
		viewPointMarker : null,

		// object for LatLngs by distance traveled (for highcharts hover values)
		pointsByDistanceTraveled : {},

		// if the activity is a live tracking activity or not
		isLive : false,

		// photos / status update popup
		statusUpdatePopup : null,

		// statusUpdates
		statusUpdates : [],

		// points from the start
		initialPoints : null,

		// for serialized deltatimes of edited points
		paceByDistanceRange : {},

		// total time spent paused
		pauseTime : 0,

		editActivity : false,

		mapboxAccessToken: 'pk.eyJ1Ijoic2ltb25kYXZhbGwiLCJhIjoiY2t2dHFuNXFxMnpmYjJ4b3VtcXVjODVkeiJ9.icCJP9MPW4BrfnTz5-y4Yg'
	},
	initialize	: function(options)
	{
		L.mapbox.accessToken = mapController.model.mapboxAccessToken;
		mapController.model.snapToRoads = options.snapToRoads;
		mapController.model.distanceUnits = options.distanceUnits;
		
		if(options.editMode)
		{
			mapController.markerOptions.startMarker.icon = new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/edit_start.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,15]
			});
			mapController.markerOptions.pauseMarker.icon = new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/edit_pause.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,15]
			});;
			mapController.markerOptions.resumeMarker.icon = new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/edit_resume.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,15]
			});;
			mapController.markerOptions.finishMarker.icon = new L.icon({
				'iconUrl'       : siteRoot + '/images/map-icons/edit_end.png',
				'iconSize' 		: [30,30],
				'iconAnchor' 	: [15,15]
			});;
		}

		if(typeof(options.editActivity)!=='undefined')
		{
			mapController.model.editActivity = options.editActivity;
		}

		if(typeof(options.points)!=='undefined')
		{
			mapController.model.initialPoints = options.points;
		}

		if(typeof(options.isLive)!=='undefined')
		{
			mapController.model.isLive = options.isLive;
			if(mapController.model.isLive)
			{
				mapController.markerOptions.finishMarker.icon = mapController.markerOptions.viewPointMarker.icon;
			}
		}

		if(typeof(options.lineColor)!=='undefined')
		{
			mapController.model.lineColor = options.lineColor;
		}
		else
		{
			mapController.model.lineColor = '#ff0000';
		}

		// create map (default location is boston)
		mapController.model.map = new L.mapbox.map(options.mapDiv).setView([42.3601, -71.0589], 1);
		L.mapbox.styleLayer('mapbox://styles/mapbox/outdoors-v11').addTo(mapController.model.map);

		L.control.layers({
			'Street View'		: L.mapbox.styleLayer('mapbox://styles/mapbox/streets-v11'),
			'Terrain View'		: L.mapbox.styleLayer('mapbox://styles/mapbox/outdoors-v11'),
			'Satellite View' 	: L.mapbox.styleLayer('mapbox://styles/mapbox/satellite-streets-v11')
		}).addTo(mapController.model.map);

		//todo - sdv removed as no fullscreen function
		//L.control.fullscreen().addTo(mapController.model.map);

		$('#map-loading').fadeOut(400,function(){
			$(this).css('display','none');
		});

		mapController.mapSetup(options);
		mapController.model.map.on('zoomend', function() {
			if(options.editMode)
			{
				mapController.removeGhostPoints();
			}

			mapController.redrawKeyPoints();
			mapController.rebindKeyPoints();

			if(options.editMode)
			{
				mapController.drawGhostPoints();
			}
		});

		if(options.editMode)
		{
			mapController.onMapEdited(function(){
				mapController.removeGhostPoints();
				mapController.drawGhostPoints();
				mapController.rebindKeyPoints();
			});
		}
	},
	mapSetup : function(options)
	{
		if(typeof(options.points) !== 'undefined' && options.points !== null)
		{
			// establish key points to edit with
			mapController.setupPoints(options);

			// draw trip polyline, add to map
			mapController.redrawTripLine();
			
			// if there are bounds in the options, fit to them, otherwise, fit to the current line
			if(typeof(options.bounds) !== 'undefined')
			{
				mapController.model.map.fitBounds(options.bounds);
			}
			else
			{
				mapController.fitMapBounds();
			}

		}
		else
		{
			// user starting to edit a new map, center it where appropriate
			if(typeof(options.mapCenter)!=='undefined' && typeof(options.mapZoom)!=='undefined')
			{
				// we have a place to start
				mapController.model.map.setView(options.mapCenter,options.mapZoom);
			}
			else
			{
				// worldmap
				mapController.model.map.fitWorld();
			}
		}

		if(options.editMode)
		{
			// Add a listener for the click event
			mapController.model.map.on("click", mapController.addKeyPointAtClickLoc);

			// Add ghost points
			mapController.drawGhostPoints();
		}
	},
	setupPoints : function(options)
	{
		if(options.editMode == false)
		{
			// editing is NOT enabled -> change marker options to be invisible and immovable
			for (type in mapController.markerOptions)
			{
				mapController.markerOptions[type].draggable = false;
				if(type === 'ghostMarker' || type === 'normalMarker')
				{
					mapController.markerOptions[type].opacity = 0;
				}
			}
		}

		var points = [];
		var arrIndex = 0;
		var totalDistance = 0;

		mapController.model.pauseTime = 0;

		mapController.model.linePoints = [];

		if(options.points.length > 0)
		{
			for(var i=0;i<options.points.length;i++)
			{
				var deltaTime,deltaPause;
				if(typeof(options.points[i].deltaTime)==='undefined' || options.points[i].deltaTime == null)
				{
					deltaTime = '';
				}
				else
				{
					deltaTime = options.points[i].deltaTime;
				}
				if(typeof(options.points[i].deltaPause)==='undefined' || options.points[i].deltaPause == null)
				{
					deltaPause = 0;
				}
				else
				{
					deltaPause = options.points[i].deltaPause;
				}
				mapController.model.linePoints.push(new rkPoint(L.latLng(options.points[i].latitude,options.points[i].longitude),deltaTime,deltaPause,null));

				// if we're not in edit mode, then add the special markers now
				if(!options.editMode)
				{
					switch(i)
					{
						case 0:
							mapController.model.linePoints[i].keyPoint = new L.marker(mapController.model.linePoints[i].latLng,mapController.markerOptions.startMarker);
							mapController.model.linePoints[i].keyPoint.addTo(mapController.model.map);
						break;
						case options.points.length-1:
							mapController.model.linePoints[i].keyPoint = new L.marker(mapController.model.linePoints[i].latLng,mapController.markerOptions.startMarker);
							mapController.model.linePoints[i].keyPoint.addTo(mapController.model.map);
						break;
						default:
							if(mapController.model.linePoints[i].deltaPause > 0)
							{
								mapController.model.linePoints[i-1].keyPoint = new L.marker(mapController.model.linePoints[i-1].latLng,mapController.markerOptions.pauseMarker);
								mapController.model.linePoints[i].keyPoint = new L.marker(mapController.model.linePoints[i].latLng,mapController.markerOptions.resumeMarker);
								mapController.model.linePoints[i-1].keyPoint.addTo(mapController.model.map);
								mapController.model.linePoints[i].keyPoint.addTo(mapController.model.map);
							}
						break;
					}
				}
			}
		}

		// if we're in edit mode, setup key points with events
		if(options.editMode)
		{
			mapController.redrawKeyPoints(options.editMode);
		}
	},
	getNextKeyPointIndex : function(fromIndex)
	{
		// discover index of next keypoint
		for(var i=fromIndex+1;i<mapController.model.linePoints.length;i++)
		{
			var thisLinePoint = mapController.model.linePoints[i];
			if(thisLinePoint.keyPoint != null)
			{
				return i;
			}
		}

		return fromIndex;
	},
	getPrevKeyPointIndex : function(fromIndex)
	{
		// discover index of next keypoint
		if(i!=0)
		{
			for(var i=fromIndex-1;i>=0;i--)
			{
				var thisLinePoint = mapController.model.linePoints[i];
				if(thisLinePoint.keyPoint != null)
				{
					return i;
				}
			}
		}

		return fromIndex;
	},
	clearKeyPoints : function()
	{
		for(var j=0;j<mapController.model.linePoints.length;j++)
		{
			var thisLinePoint = mapController.model.linePoints[j];
			if(thisLinePoint.keyPoint != null)
			{
				mapController.model.map.removeLayer(thisLinePoint.keyPoint);
				thisLinePoint.keyPoint = null;
			}
		}
	},
	redrawKeyPoints : function(editMode)
	{
		// clear key points
		mapController.clearKeyPoints();

		// loop through, if the point should be a keypoint, set the keypoint variable and add to map
		var distanceSinceLastKeyPoint = 0;
		for(var j=0;j<mapController.model.linePoints.length;j++)
		{
			var thisLinePoint = mapController.model.linePoints[j];
			var markerOptions = null;

			if(j==0)
			{
				// set start point options
				markerOptions = mapController.markerOptions.startMarker;
			}
			else if(j==mapController.model.linePoints.length-1)
			{
				// set finish point options
				markerOptions = mapController.markerOptions.finishMarker;
			}
			else if(mapController.model.linePoints[j+1].deltaPause > 0)
			{
				// set pause point options
				markerOptions = mapController.markerOptions.pauseMarker;
			}
			else if(thisLinePoint.deltaPause > 0)
			{
				// set resume point options
				markerOptions = mapController.markerOptions.resumeMarker;
			}
			else
			{
				// normal point, update distance variables
				var prevLinePoint = mapController.model.linePoints[j-1];
				var distanceBetweenPoints = distanceFunctions.distHaversine(prevLinePoint.latLng,thisLinePoint.latLng);
				distanceSinceLastKeyPoint += distanceBetweenPoints;

				// if more than X meters have passed since last key point, make a normal key point.
				// otherwise, simply kick the distance since last key point variable
				if(distanceSinceLastKeyPoint > mapController.getZoomDictionary(mapController.model.map.getZoom()).markerDist)
				{
					markerOptions = mapController.markerOptions.normalMarker;

					distanceSinceLastKeyPoint = 0;
				}
			}

			if(markerOptions != null)
			{
				thisLinePoint.keyPoint = new L.marker(thisLinePoint.latLng,markerOptions);
				thisLinePoint.keyPoint.addTo(mapController.model.map);
			}
		}

		mapController.redrawTripLine();
	},
	removeLinePointsBetween : function(startIndex,endIndex)
	{
		if(startIndex == null || typeof(startIndex)==='undefined' || endIndex == null || typeof(endIndex)==='undefined')
		{
			return [];
		}
		var totalToDelete = endIndex - startIndex - 1;
		var deletedPoints = mapController.model.linePoints.splice(startIndex+1,totalToDelete);
		return deletedPoints;
	},
	rebindKeyPoints : function()
	{
		for(var a=0;a<mapController.model.linePoints.length;a++)
		{
			if(mapController.model.linePoints[a].keyPoint != null)
			{
				mapController.addKeyPointEventHandlers(a);
			}
		}
	},
	addKeyPointEventHandlers : function(linePointIndex)
	{
		var thisLinePoint = mapController.model.linePoints[linePointIndex];

		// event handlers for dragstart,dragend, drag
		var deletedPoints,prevKeyPointIndex,nextKeyPointIndex;

		var deletedPointsBefore = null;
		var deletedPointsAfter = null;

		thisLinePoint.keyPoint.off('dragstart').off('dragend').off('drag').off('contextmenu');
		thisLinePoint.keyPoint
			.on(
				'dragstart',
				function(){
					mapController.pushUndoSnapShot();

					// start dragging (avoid click events)
					mapController.startedDragging();
					// get next keypoint line point index, previous keypoint line point index
					nextKeyPointIndex = mapController.getNextKeyPointIndex(linePointIndex);
					prevKeyPointIndex = mapController.getPrevKeyPointIndex(linePointIndex);
					if(prevKeyPointIndex != null)
					{
						// delete points before
						deletedPointsBefore = mapController.removeLinePointsBetween(prevKeyPointIndex,linePointIndex);
						// points have been removed, adjust index references
						linePointIndex -= deletedPointsBefore.length;
						nextKeyPointIndex -= deletedPointsBefore.length;
					}

					if(nextKeyPointIndex != null)
					{
						// delete points after
						deletedPointsAfter = mapController.removeLinePointsBetween(linePointIndex,nextKeyPointIndex);
						// points have been removed, adjust index references
						nextKeyPointIndex -= deletedPointsAfter.length;
					}
				}
			)
			.on(
				'drag',
				function(){
					mapController.redrawTripLine();
					thisLinePoint.latLng = thisLinePoint.keyPoint.getLatLng();
				}
			)
			.on(
				'dragend',
				function(){
					// we are either already set, or need to fill with snap to roads points
					if(mapController.model.snapToRoads && mapController.model.linePoints.length>1)
					{
						// nothing before starting point / nothing before a resume point
						// nothing after end point / nothing after a pause point
						// otherwise, use this point as waypoint

						var startingPoint,endingPoint,waypoint,callback;
						if(linePointIndex == 0 || mapController.model.linePoints[linePointIndex].deltaPause > 0)
						{
							// this is a start or resume point, only show stuff after
							startingPoint = mapController.model.linePoints[linePointIndex].latLng;
							endingPoint = mapController.model.linePoints[nextKeyPointIndex].latLng;
							waypoint = null;
							callback = function(pointArray){
								if(typeof(pointArray)!=='undefined')
								{
									var points = [];
									for(var n=0;n<pointArray.length;n++)
									{
										points.push(new rkPoint(pointArray[n],'',0,null));
									}
									for(var j=0;j<points.length;j++)
									{
										mapController.model.linePoints.splice(linePointIndex+1+j,0,points[j]);
									}
								}

								mapController.redrawTripLine();
								mapController.mapEditedFuncs();
							};
						}
						else if(linePointIndex == mapController.model.linePoints.length-1 || mapController.model.linePoints[linePointIndex+1].deltaPause > 0)
						{
							// this is a finish or pause point, only show stuff before
							startingPoint = mapController.model.linePoints[prevKeyPointIndex].latLng;
							endingPoint = mapController.model.linePoints[linePointIndex].latLng;
							waypoint = null;
							callback = function(pointArray){
								if(typeof(pointArray)!=='undefined')
								{
									var points = [];
									for(var n=0;n<pointArray.length;n++)
									{
										points.push(new rkPoint(pointArray[n],'',0,null));
									}
									for(var j=0;j<points.length;j++)
									{
										mapController.model.linePoints.splice(prevKeyPointIndex+1+j,0,points[j]);
									}
								}
								mapController.redrawTripLine();
								mapController.mapEditedFuncs();
							};
						}
						else {
							// normal, move with waypoint
							startingPoint = mapController.model.linePoints[prevKeyPointIndex].latLng;
							endingPoint = mapController.model.linePoints[nextKeyPointIndex].latLng;
							waypoint = mapController.model.linePoints[linePointIndex].latLng;
							callback = function(pointArray){
								if(typeof(pointArray)!=='undefined')
								{
									var createdReplacementPoint = false;
									for(var i=0;i<pointArray.length;i++)
									{
										var numberToOverwrite = 0;
										var newKeypoint = null;
										if(i > (pointArray.length / 2) && !createdReplacementPoint)
										{
											newKeypoint = new L.marker(pointArray[i],mapController.markerOptions.normalMarker);
											newKeypoint.addTo(mapController.model.map);
											createdReplacementPoint = true;
										}
										if(i === pointArray.length - 1)
										{
											numberToOverwrite = 1;
											mapController.model.map.removeLayer(mapController.model.linePoints[prevKeyPointIndex+1+i].keyPoint);
										}
										mapController.model.linePoints.splice(prevKeyPointIndex+1+i,numberToOverwrite,new rkPoint(pointArray[i],'',0,newKeypoint));
									}
								}
								mapController.redrawTripLine();
								mapController.mapEditedFuncs();
							};
						}

						mapController.getSnapToRoadsRoute(
							startingPoint,
							endingPoint,
							waypoint,
							callback
						);
					}
					else
					{
						mapController.redrawTripLine();
						mapController.mapEditedFuncs();
					}

					// done dragging
					mapController.finishedDragging();
				}
			)
		;
	

		// bind delete element on right click
		thisLinePoint.keyPoint.on('contextmenu',function(){
			mapController.pushUndoSnapShot();
			var prevKeyPointIndex = mapController.getPrevKeyPointIndex(linePointIndex);
			var nextKeyPointIndex = mapController.getNextKeyPointIndex(linePointIndex);
			var deletePointAsWell = (linePointIndex == nextKeyPointIndex || linePointIndex == prevKeyPointIndex);
			mapController.removeLinePointsBetween(prevKeyPointIndex,nextKeyPointIndex);
			var isFirstPoint = (linePointIndex === 0);
			var isLastPoint = (mapController.model.linePoints.length-1 === linePointIndex);
			linePointIndex -= (linePointIndex-prevKeyPointIndex);
			if(deletePointAsWell)
			{
				if(isFirstPoint)
				{
					mapController.model.linePoints.splice(0,1);
				}
				else if(isLastPoint)
				{
					mapController.model.linePoints.splice(mapController.model.linePoints.length-1,1);
				}
				else
				{
					mapController.model.linePoints.splice(linePointIndex+1,1);
				}
			}
			mapController.model.map.removeLayer(thisLinePoint.keyPoint);
			mapController.redrawKeyPoints(true);
			mapController.mapEditedFuncs();
		});
	},
	// takes in 2 points and a stopover
	getSnapToRoadsRoute : function(startPointLatLng,endPointLatLng,wayPointLatLng,callback)
	{
		var routingUrl;
		var pointsArray = [];
		
		if(wayPointLatLng !== null && typeof(wayPointLatLng)!== 'undefined')
		{
			pointsArray = [startPointLatLng,wayPointLatLng,endPointLatLng];
		}
		else
		{
			pointsArray = [startPointLatLng,endPointLatLng];
		}

		$.ajax({
			url : mapController.getDirectionsAPIUrl(pointsArray),
			success : function(data)
			{
				// loop through routes
				if(data.routes.length > 0)
				{
					var thisRoute = [];
					var decodedArray = polyline.decode(data.routes[0].geometry);

					// loop through steps, create latlng, add
					for(var j=0;j<decodedArray.length;j++)
					{
						var newPointLatLng = [decodedArray[j][0]/10,decodedArray[j][1]/10];
						thisRoute.push(new L.LatLng(newPointLatLng[0],newPointLatLng[1]));
					}

					callback(thisRoute);
				}
				else
				{
					callback();
				}
			},
			error : function(data)
			{
				console.log('error : ');
				console.log(data);
				callback();
			}
		});
	},
	startedDragging				: function()
	{
		mapController.model.justFinishedDragging = true;
	},
	finishedDragging 			: function()
	{
		setTimeout(function(){
			mapController.model.justFinishedDragging = false;
		},50);
	},
	processImportData : function(points)
	{
		// clear any map data
		mapController.reset();

		// add imported points
		mapController.setupPoints({
			'points' : points,
			'editMode' : true
		});

		// run any edited functions
        mapController.mapEditedFuncs();
	},
	drawRaceLine : function(points,overrideTrip)
	{
		var pointsArray = [];
		for(var i=0;i<points.length;i++)
		{
			var deltaTime,deltaPause;
			if(typeof(points[i].deltaTime)==='undefined' || points[i].deltaTime == null)
			{
				deltaTime = '';
			}
			else
			{
				deltaTime = points[i].deltaTime;
			}
			if(typeof(points[i].deltaPause)==='undefined' || points[i].deltaPause == null)
			{
				deltaPause = 0;
			}
			else
			{
				deltaPause = points[i].deltaPause;
			}

			// recreate the points as javascript object
			points[i] = new rkPoint(new L.latLng(points[i].latitude,points[i].longitude),deltaTime,deltaPause,null);

			// get points to plug into the polyline
			pointsArray.push(points[i].latLng);
		}

		// if we want this race line to ALSO be the trip line
		if(overrideTrip)
		{
			// clear the map
			mapController.reset();

			// set line points to what we just created
			mapController.model.linePoints = points;

			// setup the key points at this zoom level
			mapController.redrawKeyPoints(true);
		}

		// race polyline, set the points to the array of latlngs we created before
		if(mapController.model.racePolyline !== null)
		{
			mapController.model.racePolyline.setLatLngs(pointsArray);
		}
		else
		{
			mapController.model.racePolyline = new L.polyline(pointsArray,{'color' : '#2c7c2c','opacity' : .5,'weight' : 4});
			mapController.model.racePolyline.addTo(mapController.model.map);
		}

		// fit map, run edited funcs
		mapController.fitMapBounds();
		mapController.mapEditedFuncs();
	},
	drawRouteLine : function(points,overrideTrip)
	{
		var pointsArray = [];
		for(var i=0;i<points.length;i++)
		{
			var deltaTime,deltaPause;
			if(typeof(points[i].deltaTime)==='undefined' || points[i].deltaTime == null)
			{
				deltaTime = '';
			}
			else
			{
				deltaTime = points[i].deltaTime;
			}
			if(typeof(points[i].deltaPause)==='undefined' || points[i].deltaPause == null)
			{
				deltaPause = 0;
			}
			else
			{
				deltaPause = points[i].deltaPause;
			}

			// recreate the points as javascript object
			points[i] = new rkPoint(new L.latLng(points[i].latitude,points[i].longitude),deltaTime,deltaPause);

			// get points to plug into the polyline
			pointsArray.push(points[i].latLng);
		}

		// if we want this route line to ALSO be the trip line
		if(overrideTrip)
		{
			// clear the map
			mapController.reset();

			// set line points to what we just created
			mapController.model.linePoints = points;

			// setup the key points at this zoom level
			mapController.redrawKeyPoints(true);
		}

		// route polyline, set the points to the array of latlngs we created before
		if(mapController.model.routePolyline !== null)
		{
			mapController.model.routePolyline.setLatLngs(pointsArray);
		}
		else
		{
			mapController.model.routePolyline = new L.polyline(pointsArray,{'color' : '#0000ff','opacity' : .5,'weight' : 4});
			mapController.model.routePolyline.addTo(mapController.model.map);
		}

		// fit map, run edited funcs
		mapController.fitMapBounds();
		mapController.mapEditedFuncs();
	},
	fitMapBounds : function()
	{
		// get the map bounds by looking at all polylines involved
		var bounds = new L.LatLngBounds();

		// activity polylines
		for(var i=0; i<mapController.model.activityPolylines.length;i++)
		{
			bounds = bounds.extend(mapController.model.activityPolylines[i].getBounds());
		}

		// route polyline
		if(mapController.model.routePolyline !==null)
		{
			bounds = bounds.extend(mapController.model.routePolyline.getBounds());
		}

		// race polyline
		if(mapController.model.racePolyline !==null)
		{
			bounds = bounds.extend(mapController.model.racePolyline.getBounds());
		}

		// fit to the overall bounds
		mapController.model.map.fitBounds(bounds);
	},
	getZoomDictionary : function(zoomLevel)
	{
		if(zoomLevel >= 17)
		{
			return {
				'ghostDist' : 10,
				'markerDist' : 10,
			};
		}
		else if(zoomLevel == 16)
		{
			return {
				'ghostDist' : 20,
				'markerDist' : 50,
			};
		}
		else if(zoomLevel == 15)
		{
			return {
				'ghostDist' : 40,
				'markerDist' : 200,
			};
		}
		else if(zoomLevel == 14)
		{
			return {
				'ghostDist' : 60,
				'markerDist' : 300,
			};	
		}
		else if (zoomLevel < 12)
		{
			return {
				'ghostDist' : 500,
				'markerDist' : 1000,
			};		
		}
		else if (zoomLevel < 14)
		{
			return {
				'ghostDist' : 100,
				'markerDist' : 500,
			};		
		}
	},
	removeGhostPoints : function()
	{
		for(var i=0;i<mapController.model.ghostPoints.length;i++)
		{
			mapController.model.map.removeLayer(mapController.model.ghostPoints[i].marker);
		}
		mapController.model.ghostPoints = [];
	},
	drawGhostPoints : function()
	{
		/*
			Ghost points:
				we want to be able to create these on the fly, as we draw the line.
				invisible points all over the line, at specific intervals, on which you can hover, click + drag to create a new keypoint
				distance between points varies based on zoom level
		*/

		mapController.removeGhostPoints();

		// draw a fake point in between every key point to allow user to insert a new point here.
		if(mapController.model.linePoints.length>1)
		{
			// variable for distance since last ghostpoint
			var distanceSinceLastGhostPoint = 0;

			// how often SHOULD we be placing ghost points?
			var ghostPointInterval = mapController.getZoomDictionary(mapController.model.map.getZoom()).ghostDist;

			// after placing a ghost point, how much distance is left over (so we can place correctly after the next invisible point)
			var carryoverDistance = 0;
			var lastKeyPointIndex = 0;
			for(var j=0; j<mapController.model.linePoints.length;j++)
			{
				var thisLinePoint, prevLinePoint; 
				thisLinePoint = mapController.model.linePoints[j];

				// if we're dealing with a resume point, skip to the next one.
				if(thisLinePoint.deltaPause > 0)
				{
					continue;
				}

				// variable for point-to-point distance
				var distanceBetween = 0;
				if(j>0)
				{
					prevLinePoint = mapController.model.linePoints[j-1];
					distanceBetween = distanceFunctions.distHaversine(prevLinePoint.latLng,thisLinePoint.latLng);
					distanceSinceLastGhostPoint += distanceBetween;
				}

				// if we've traveled more than the ghost point interval, find out how many to place
				var ghostPointsToPlace = 0;
				if(distanceSinceLastGhostPoint > ghostPointInterval)
				{
					ghostPointsToPlace = Math.floor(distanceSinceLastGhostPoint / ghostPointInterval);
				}
				
				// Sanity check the # of ghost points to insert. If it's ridiculously large we'll need to increase the interval between them
				// since we'll just hang / crash the browser if we try to insert hundreds of thousands of ghost points. This crops up when
				// GPX / TCX files have "bad" data such as points at 0,0 lat,lon.
				if (ghostPointsToPlace > mapController.MAX_GHOST_POINTS)
				{
				   // Make the # of points fixed and compute the distance between instead so we can limit the # of ghost points...
				   ghostPointsToPlace = mapController.MAX_GHOST_POINTS;
				   ghostPointInterval = Math.floor(distanceSinceLastGhostPoint / ghostPointsToPlace);
				}

				// start with carryover distance
				// move from point A to point B, placing ghost points at interval specified (remove carryover from last loop iteration)
				// when done, set carryover distance and continue
				if(ghostPointsToPlace > 0)
				{
					for(var k=1;k<=ghostPointsToPlace;k++)
					{
						var ghostPointLat, ghostPointLng;
						var distanceToNextGhostPoint = (ghostPointInterval * k) - carryoverDistance;
						var percentageBetweenPoints = distanceToNextGhostPoint / distanceBetween;

						// we now know at what percentage distance the ghost point should be placed. Use to put it on the line
						ghostPointLat = prevLinePoint.latLng.lat + ((thisLinePoint.latLng.lat-prevLinePoint.latLng.lat)*percentageBetweenPoints);
						ghostPointLng = prevLinePoint.latLng.lng + ((thisLinePoint.latLng.lng-prevLinePoint.latLng.lng)*percentageBetweenPoints);

						// create GhostPoint object
						var newGhostPoint = new GhostPoint(
							new L.marker(
								new L.latLng(ghostPointLat,ghostPointLng),
								mapController.markerOptions.ghostMarker
							),
							lastKeyPointIndex
						);

						// add to map
						newGhostPoint.marker.addTo(mapController.model.map);

						// add to array for reference
						mapController.model.ghostPoints.push(newGhostPoint);

						// set up event handlers
						mapController.addGhostPointEventHandlers(newGhostPoint);

						if(k == ghostPointsToPlace)
						{
							// last need
							carryoverDistance = distanceBetween - (percentageBetweenPoints * distanceBetween);
							distanceSinceLastGhostPoint = carryoverDistance;
						}
					}
				}
				else
				{
					carryoverDistance+=distanceBetween;
				}

				if(thisLinePoint.keyPoint != null)
				{
					lastKeyPointIndex = j;
				}
			}
		}
	},
	addGhostPointEventHandlers : function(ghostPoint)
	{
		var linePointIndex = ghostPoint.linePointIndex;
		var deletedPoints,createdPoint;

		var nextKeyPointIndex = mapController.getNextKeyPointIndex(linePointIndex);
		var prevKeyPointIndex = mapController.getPrevKeyPointIndex(linePointIndex)
		// events!
		ghostPoint.marker.off('mouseover').off('mouseout').off('click').off('drag').off('dragstart').off('dragend')
			.on(
				'mouseover',
				function()
				{
					// show it
					this.setOpacity(0.7);
				}
			)
			.on(
				'mouseout',
				function()
				{
					// hide it
					this.setOpacity(0);
				}
			).on(
				'dragstart',
				function()
				{
					mapController.pushUndoSnapShot();
					// remove points between the two nearest keypoints
					// create fake point for line redraw
					// on drag end, connect the dots

					// remove the points
					deletedPoints = mapController.removeLinePointsBetween(linePointIndex,nextKeyPointIndex);
					createdPoint = new rkPoint(ghostPoint.marker.getLatLng(),'',0,new L.marker(ghostPoint.marker.getLatLng(),mapController.markerOptions.normalMarker));
					mapController.model.linePoints.splice(linePointIndex+1,0,createdPoint);
					linePointIndex++;
					nextKeyPointIndex++;
				}
			).on(
				'drag',
				function(){
					createdPoint.latLng = ghostPoint.marker.getLatLng();
					createdPoint.keyPoint.setLatLng(createdPoint.latLng);
					mapController.redrawTripLine();
				}
			).on(
				'dragend',
				function(){
					createdPoint.keyPoint.addTo(mapController.model.map);
					if(mapController.model.snapToRoads && mapController.model.linePoints.length>1)
					{
						// nothing before starting point / nothing before a resume point
						// nothing after end point / nothing after a pause point
						// otherwise, use this point as waypoint

						// normal, move with waypoint
						var startingPoint = mapController.model.linePoints[linePointIndex-1].latLng;
						var endingPoint = mapController.model.linePoints[linePointIndex+1].latLng;
						var waypoint = mapController.model.linePoints[linePointIndex].latLng;
						var callback = function(pointArray){
							if(typeof(pointArray)!=='undefined')
							{
								var prevKeyPointIndex = mapController.getPrevKeyPointIndex(linePointIndex);
								var createdReplacementPoint = false;
								for(var i=0;i<pointArray.length;i++)
								{
									var numberToOverwrite = 0;
									var newKeypoint = null;
									if(i > (pointArray.length / 2) && !createdReplacementPoint)
									{
										newKeypoint = new L.marker(pointArray[i],mapController.markerOptions.normalMarker);
										newKeypoint.addTo(mapController.model.map);
										createdReplacementPoint = true;
									}

									if(i === pointArray.length - 1)
									{
										numberToOverwrite = 1;
										mapController.model.map.removeLayer(mapController.model.linePoints[prevKeyPointIndex+1+i].keyPoint);
									}

									mapController.model.linePoints.splice(prevKeyPointIndex+1+i,numberToOverwrite,new rkPoint(pointArray[i],'',0,newKeypoint));
								}
							}
							mapController.redrawTripLine();
							mapController.mapEditedFuncs();
						};

						mapController.getSnapToRoadsRoute(
							startingPoint,
							endingPoint,
							waypoint,
							callback
						);
					}
					else
					{
						mapController.redrawTripLine();
						mapController.mapEditedFuncs();
					}
				}
			);
	},
	redrawTripLine : function()
	{
		// remove markers
		mapController.removeDistanceMarkers();

		// clear pointsByDistanceTraveled object (used by highCharts)
		mapController.model.pointsByDistanceTraveled = {};

		// set initial 'total Distance' variable
		var totalDistance = 0;

		// remove all activity line polylines from the map
		for(var i=0;i<mapController.model.activityPolylines.length;i++)
		{
			mapController.model.map.removeLayer(mapController.model.activityPolylines[i]);
		}

		// do the same for the thumbnail map, if applicable
		if(mapController.model.thumbnailMap)
		{
			for(var i=0;i<mapController.model.thumbnailPolylines.length;i++)
			{
				mapController.model.thumbnailMap.removeLayer(mapController.model.thumbnailPolylines[i]);
			}
		}

		// clear both arrays for re-use
		mapController.model.activityPolylines = [];
		mapController.model.thumbnailPolylines = [];

		// loop through line points

		// distance variables for lines
		var lastlineStartPointIndex = 0;
		var lastLinePointTotalDistance = 0;

		// what distance marker we are currently on
		var markerCount = 0;
		for(var i=0;i<mapController.model.linePoints.length;i++)
		{
			var thisLinePoint = mapController.model.linePoints[i];
			var thisLegDist = 0;

			// if we have reached a pause point OR the last point (finish) draw this segment and process
			if(i == mapController.model.linePoints.length-1 || mapController.model.linePoints[i+1].deltaPause > 0)
			{
				// draw this line segment
				var nextLine = [];
				for(var j=lastlineStartPointIndex;j<=i;j++)
				{
					var thisLinePointLatLng = mapController.model.linePoints[j].latLng;
					var prevLinePointLatLng;

					nextLine.push(mapController.model.linePoints[j].latLng);

					if(j != lastlineStartPointIndex)
					{
						prevLinePointLatLng = mapController.model.linePoints[j-1].latLng;
						thisLegDist = distanceFunctions.flooredNum(distanceFunctions.distHaversine(prevLinePointLatLng,thisLinePointLatLng),6);
						totalDistance = distanceFunctions.flooredNum(totalDistance+thisLegDist,6);
					}

					// handle the total distance stamp for Highcharts point mapping
					var distKeyString = totalDistance / 1000;
					if (mapController.model.distanceUnits === 'mi') distKeyString /= distanceFunctions.KM_PER_MILE;
					var curDist = distKeyString;
					distKeyString = distanceFunctions.flooredNum(distKeyString,3);
					var distKey = ("d" + (distKeyString)).replace(".", "p");
					mapController.model.pointsByDistanceTraveled[distKey] = thisLinePointLatLng;

					// are we 1 distance unit (or more) greater? try to draw a distance marker
					if(curDist >= markerCount + 1)
					{
						var markersToAdd = Math.floor(curDist) - markerCount;
						for(var k=0; k < markersToAdd; k++)
						{
							// kick marker number
							markerCount++;

							// if zoomed in far enough, show all individual mile markers
							// if not zoomed in past 13, show every 5 if at 12
							// if not zoomed in past 12, show every 10
							if ((mapController.model.map.getZoom() >= 13) || ((mapController.model.map.getZoom() >= 12) && (markerCount % 5 == 0)) || (markerCount % 10 == 0))
							{
								// lat lng of distance marker to place
								var distPointLat;
								var distPointLng;

								// find lat lng
								
								var distanceToDistMarker = markerCount - lastLinePointTotalDistance;
								var percentageDist;
								if (mapController.model.distanceUnits === 'mi')
								{
									percentageDist = distanceToDistMarker / (thisLegDist / 1000 / distanceFunctions.KM_PER_MILE);
								}
								else
								{
									percentageDist = distanceToDistMarker / (thisLegDist / 1000);
								}
								distPointLat = prevLinePointLatLng.lat + ((thisLinePointLatLng.lat-prevLinePointLatLng.lat)*percentageDist);
								distPointLng = prevLinePointLatLng.lng + ((thisLinePointLatLng.lng-prevLinePointLatLng.lng)*percentageDist);

								var textSizeChange = '';
								if(markerCount > 99)
								{
									textSizeChange = ' style="font-size: 9px;"';
								}

								// set up icon + marker
								var distMarker = new L.marker(
									new L.latLng(distPointLat,distPointLng)
									,
									{
										'icon' : new L.divIcon({
											className: 'mileMarker',
											html: '<div class="number"' + textSizeChange + '>' + markerCount + '</div><div class="distUnit">' + mapController.model.distanceUnits + '</div></div>',
											iconSize: [40, 40],
											iconAnchor: [20, 35]
			                             }),
										'clickable'	: false,
										'draggable' : false,
										'opacity'	: 1,
										'zIndexOffset' : 498
									}
								);

								// push to distance marker array
								mapController.model.distanceMarkers.push(distMarker);

								// draw this distance marker on map
								distMarker.addTo(mapController.model.map);
							}
						}
					}
					
					// update last line point distance stamp
					lastLinePointTotalDistance = curDist;
				}

				// draw lines on map and thumbnails in the set color
				var thisLine = new L.polyline(nextLine,{'color':mapController.model.lineColor});
				var thisThumbnailLine = new L.polyline(nextLine,{'color':mapController.model.lineColor});
				
				thisLine.addTo(mapController.model.map);
				if(mapController.model.thumbnailMap)
				{
					thisThumbnailLine.addTo(mapController.model.thumbnailMap);
				}
				// add polylines to their respective arrays, for reference
				mapController.model.thumbnailPolylines.push(thisThumbnailLine);
				mapController.model.activityPolylines.push(thisLine);

				// kick last line startpoint index (start at the next point, the resume)
				lastlineStartPointIndex = i+1;
			}
		}

		// distance to KM
		mapController.model.distance = totalDistance / 1000;

		if (mapController.model.distanceUnits === 'mi') mapController.model.distance /= distanceFunctions.KM_PER_MILE;

		// run distanceupdated function stack
		if (mapController.model.distanceUpdated) {           
			mapController.model.distanceUpdated(mapController.model.distance);
		}
	},
	thumbnailMapBounds : function()
	{
		if(mapController.model.thumbnailPolylines.length > 0)
		{
			var thumbnailBounds = null;

			for(var i=0;i<mapController.model.thumbnailPolylines.length;i++)
			{
				if(thumbnailBounds === null)
				{
					thumbnailBounds = mapController.model.thumbnailPolylines[i].getBounds();
				}
				else
				{
					thumbnailBounds.extend(mapController.model.thumbnailPolylines[i].getBounds());
				}
			}

			mapController.model.thumbnailMap.fitBounds(thumbnailBounds);
		}
		else
		{
			mapController.model.thumbnailMap.fitBounds(mapController.model.map.getBounds());	
		}
	},
	addKeyPointAtClickLoc : function(event)
	{
		// setup a snapshot to revert to

		mapController.pushUndoSnapShot();
		// ensure that we aren't in the process of dragging a point
		if(!mapController.model.justFinishedDragging)
		{
			// create new point, add to map, redraw the trip line

			// we don't want to use the routing api if it's only the first point on the map (there's no beginning / end)
			var firstPoint = false;
			if(mapController.model.linePoints.length == 0)
			{
				firstPoint = true;
			}

			// need to remember where in the linePoints array we're adding to, so we can prep the undo function
			var prevEndingIndex = mapController.model.linePoints.length;

			// create any points involved, add to linePoints array
			if(mapController.model.snapToRoads && !firstPoint)
			{
				mapController.getSnapToRoadsRoute(
					mapController.model.linePoints[mapController.model.linePoints.length-1].latLng,
					event.latlng,
					null,
					function(pointArray){
						if(typeof(pointArray)!=='undefined')
						{
							for(var i=0;i<pointArray.length;i++)
							{
								mapController.model.linePoints.push(new rkPoint(pointArray[i],'',0,null));
							}
							mapController.redrawKeyPoints(true);
							mapController.mapEditedFuncs();
						}
						else
						{
							// error, default to normal line behavior
							mapController.model.linePoints.push(new rkPoint(event.latlng,'',0,null));
							mapController.redrawKeyPoints(true);
							mapController.mapEditedFuncs();
						}
					}
				);
			}
			else
			{
				mapController.model.linePoints.push(new rkPoint(event.latlng,'',0,null));
				mapController.redrawKeyPoints(true);
				mapController.mapEditedFuncs();
			}
		}
	},
	removeDistanceMarkers : function()
	{
		// start by removing the ghost points already there
		for(var j=0; j<mapController.model.distanceMarkers.length; j++)
		{
			mapController.model.map.removeLayer(mapController.model.distanceMarkers[j]);
		}
		mapController.model.distanceMarkers = [];
	},
	pushUndoSnapShot : function()
	{
		var snapShot = [];
		for(var i=0;i<mapController.model.linePoints.length;i++)
		{
			var thisLinePoint = mapController.model.linePoints[i];
			snapShot.push(new rkPoint(thisLinePoint.latLng,thisLinePoint.deltaTime,thisLinePoint.deltaPause,null));
		}
		mapController.model.snapShotStack.push(snapShot);
	},
	revertToSnapshot : function(snapShotIndex)
	{
		for(var i=0;i<mapController.model.linePoints.length;i++)
		{
			var linePointToClear = mapController.model.linePoints[i].keyPoint;
			if(linePointToClear != null)
			{
				mapController.model.map.removeLayer(linePointToClear);
			}
		}
		
		if(typeof(snapShotIndex) === 'undefined' || snapShotIndex == null)
		{
			snapShotIndex = mapController.model.snapShotStack.length-1;
		}

		mapController.model.linePoints = mapController.model.snapShotStack[snapShotIndex];

		var numberToPop = mapController.model.snapShotStack.length - snapShotIndex;
		for(var i=0;i<numberToPop;i++)
		{
			mapController.model.snapShotStack.pop();
		}

		mapController.redrawKeyPoints(true);
		mapController.mapEditedFuncs();
	},
	undo : function()
	{
		if(mapController.model.snapShotStack.length == 0)
		{
			console.log("Nothing left to undo.");
		}
		else
		{
			mapController.revertToSnapshot();
		}

		if (mapController.model.distanceUpdated) {           
			mapController.model.distanceUpdated(mapController.model.distance);
		}
	},
	reset : function()
	{
		// just undo everything.
		if(mapController.model.snapShotStack.length > 0)
		{
			mapController.revertToSnapshot(0);
		}

		if (mapController.model.mapReset.length > 0) {
			for(var i=mapController.model.mapReset.length-1;i>=0;i--)
			{
				mapController.model.mapReset[i]();
			}
		}
		
		mapController.model.mapReset = [];
	},
	clear : function()
	{
		// kill all the things
		mapController.reset();
		mapController.clearKeyPoints();
		for(var i=0;i<mapController.model.activityPolylines.length;i++)
		{
			mapController.model.map.removeLayer(mapController.model.activityPolylines[i]);
		}

		mapController.model.activityPolylines = [];
		mapController.model.thumbnailPolylines = [];
		mapController.model.keyPoints = [];
		mapController.model.linePoints = [];
		mapController.model.distance = 0;

		mapController.mapEditedFuncs();
		if (mapController.model.distanceUpdated) {           
			mapController.model.distanceUpdated(mapController.model.distance);
		}
	},
	hasActivityData : function() 
	{
		return (mapController.model.linePoints.length > 0);
	},
	serializePoints : function()
	{
		// type + "," + latitude + "," + longitude + "," + deltaTime + "," + deltaPause + "," + deltaDistance + ";";

		// output format:
		var serializeString = "";

		for(var i=0;i<mapController.model.linePoints.length;i++)
		{
			var thisLinePoint = mapController.model.linePoints[i];
			var deltaDistance = 0;
			var prevLinePoint;
			if(i!=0)
			{
				prevLinePoint = mapController.model.linePoints[i-1];
				if(prevLinePoint.deltaPause == 0)
				{
					deltaDistance = distanceFunctions.distHaversine(mapController.model.linePoints[i-1].latLng,thisLinePoint.latLng);
				}
			}

			var rkPointType;
			if(i==0)
			{
				rkPointType = 'StartPoint';
			}
			else if(i==mapController.model.linePoints.length-1)
			{
				rkPointType = 'EndPoint';
			}
			else if(thisLinePoint.deltaPause > 0)
			{
				rkPointType = 'ResumePoint';	
			}
			else if(mapController.model.linePoints[i+1].deltaPause > 0)
			{
				rkPointType = 'PausePoint';
			}
			else
			{
				rkPointType = 'ManualPoint';
			}

			serializeString += rkPointType + ',' + thisLinePoint.latLng.lat + ',' + thisLinePoint.latLng.lng + ',' + thisLinePoint.deltaTime + ',' + thisLinePoint.deltaPause + ',' + deltaDistance + ';';
		}

		return serializeString;
	},
	mapEditedFuncs : function()
	{
		for(var i = 0, n = mapController.model.mapEdited.length; i < n; i++) {
			mapController.model.mapEdited[i](); //let callback know
		}
	},
	searchForLocation : function(request, response)
	{
		geoController.getGeoObject(
			request.term,
			function(featureSet)
			{
				if(!featureSet || featureSet.length == 0)
				{
					// nothing, fail.
					response({});
				}
				else
				{
					response(
						$.map(
							featureSet, 
							function (item) 
							{
								return {
									'label': item.place_name,
									'value': item.place_name,
									'bounds': (typeof(item.bbox)!== 'undefined') ? L.latLngBounds(L.latLng(item.bbox[1], item.bbox[0]),L.latLng(item.bbox[3], item.bbox[2])) : null,
									'location': L.latLng(item.center[1], item.center[0])
								}
							}
						)
					);
				}
			}
		);
	},
	onDistanceUpdated : function(f) {
		mapController.model.distanceUpdated = f;
	},
	onMapEdited : function(f) {
		mapController.model.mapEdited.push(f);
	},
	onMapReset : function(f) {
		mapController.model.mapReset.push(f);
	},
	showViewPointAtPoint : function(latLng)
	{
		if(mapController.model.viewPointMarker != null)
		{
			mapController.model.viewPointMarker.setLatLng(latLng);
		}
		else
		{
			mapController.model.viewPointMarker = new L.marker(latLng,mapController.markerOptions.viewPointMarker);
			mapController.model.viewPointMarker.addTo(mapController.model.map);
		}
	},
	removeViewPointMarker : function()
	{
		if(mapController.model.viewPointMarker !== null)
		{
			mapController.model.map.removeLayer(mapController.model.viewPointMarker);
			mapController.model.viewPointMarker = null;
		}
	},
	overlayStatusUpdates : function()
	{
		if (mapController.model.map == null) {
			return;
		}

		if (mapController.model.statusUpdatePopup != null)
		{
			mapController.model.statusUpdatePopup = L.popup();
		}

		// Loop through all the status updates
		for (var i = 0; i < mapController.model.statusUpdates.length; i++)
		{
			var statusUpdateIcon;
			var thisStatusUpdate = mapController.model.statusUpdates[i];
			if (thisStatusUpdate.photoKey == undefined) // text only
			{
				statusUpdateIcon = new L.icon({
					iconUrl: siteRoot + "/images/map/text.png"
				});
			}
			else // photo
			{
				statusUpdateIcon = new L.icon({
					iconUrl: siteRoot + "/images/map-icons/photo.png",
					iconSize: [35, 35],
					iconAnchor: [17, 17]
				});
			}

			// Create the marker point
			var markerPoint = new L.latLng(thisStatusUpdate.latitude, thisStatusUpdate.longitude);
			var statusUpdateMarker = new L.marker(markerPoint, {
				draggable: false,
				icon: statusUpdateIcon
			});

			var htmlBlock = $('<div />');

			if (thisStatusUpdate.photoKey != undefined)
			{
				htmlBlock.append($("<img class='mapPhotoThumb statusUpdatePopupPhoto' src='" + tripPhotoBaseUrl + "/" + thisStatusUpdate.photoKey + "_small.jpg' data-photoKey='" + thisStatusUpdate.photoKey + "' width='50' height='50' border='0' style='cursor:pointer'/>"));
			}

			if (thisStatusUpdate.text != undefined)
			{
				htmlBlock.append($("<div class='statusUpdatePopupText'>" + thisStatusUpdate.text + "</div>"));
			}

			if (thisStatusUpdate.photoKey != undefined)
			{
				htmlBlock.on('click',function(){
					$('#'+$(this).find('img.mapPhotoThumb').attr('data-photoKey')).click();
				});
			}

			statusUpdateMarker.addTo(mapController.model.map).bindPopup(htmlBlock[0]);

			statusUpdateMarker.on('mouseover',function(){
				this.openPopup();
			});
		}
	},
	getDirectionsAPIUrl : function(pointsArray)
	{
		var urlString = '';
		for(var i=0;i<pointsArray.length;i++)
		{
			if(urlString.length > 0)
			{
				urlString += ';';
			}
			urlString += pointsArray[i].lng + ',' + pointsArray[i].lat;
		}

		var returnString = 'https://api.mapbox.com/v4/directions/mapbox.walking/' + urlString + '.json?access_token=' + mapController.model.rkMapboxAccessToken + '&geometry=polyline';
		return returnString;
	}
}