var ActivityListManager = {
    getActivitiesByRange: function (startDate, endDate, callback) {
        var username = $("#userProfile .avatarContainer").attr("userUrl")
            , data = `userName=${username}&startDate=${startDate}`;

        if (endDate != null) {
            data += `&endDate=${endDate}`;
        }

        $.ajax({
            url: `/api/activity/activityListByDate?${data}`,
            complete:
                function(json) { //for some reason the stripes status code isn't working so success never gets called, but complete does
                    if (json.status === 200) {
                        callback(JSON.parse(json.responseText));
                    } else {
                        console.log(`Failed to load /activitiesByDateRange?${data}`, json.status, json.statusText);
                    }
                }
        });
    }
    , cleanUpMonthData: function () {
        $(".component.accordionContent > ul").empty();
    }
    , toggleMonthView: function (accordionObj, activities) {
        ActivityListManager.cleanUpMonthData();

        var activitiesStr = "";

        for (var i in activities) {
            var activityPath = activities[i].type === "CARDIO" ? "activity" : "strengthtrainingactivity"
                , url = `/${activityPath}/${activities[i].activity_id}`
                , extraText = ""
                , selectedId = $("#selectedActivity").val()
                , selected = selectedId === activities[i].activity_id ? "selected" : ""
                , strengthClass = activities[i].type === "STRENGTH_TRAINING" ? "strengthTraining" : "";

            if (activities[i].type === "CARDIO") {
                extraText = `<div class='distanceUnit'>${activities[i].distanceUnits}</div>`;
                extraText += `<div class='distance'>${activities[i].distance}</div>`;
            }

            activitiesStr += `<li class="nav-item ${selected} ${strengthClass}">`;
            activitiesStr += `<a class="nav-link" href="${url}">`;
            activitiesStr += `<span class="startDate">${activities[i].monthNum}/${activities[i].dayOfMonth}</span>`;

            if (activities[i].type === "CARDIO") {
                if (activities[i].distance === "" || activities[i].distance === "0.00") {
                    activitiesStr += `<span class="unitDistance">${activities[i].elapsedTime}</span>`;
                }
                else {
                    activitiesStr += `<span class="unitDistance">${activities[i].distance} ${activities[i].distanceUnits}.</span>`;
                }
            }

            activitiesStr += ` ${activities[i].mainText} `;

            activitiesStr += "</a></li>";
        }

        accordionObj.next().find("ul").append(activitiesStr);
    }
    , updateView: function (clickedObj) {
        var startDate = clickedObj.attr("data-date")
            , accordionObj = clickedObj
            , ajaxIndicatorClass = "accordionAjaxIndicator";
        
        clickedObj.append("<img src='../images/accordion-loader.gif' width='16' height='16' class='" + ajaxIndicatorClass + "' />");

        ActivityListManager.getActivitiesByRange(startDate, null, function (json) {
            if (json.hasOwnProperty("activities")) {
                var year = startDate.split("-")[2];
                var month = startDate.split("-")[0];
                ActivityListManager.toggleMonthView(accordionObj, json.activities[year][month]);
            } else {
                if (json.hasOwnProperty("status")) {
                    console.log("Failed request to api/activity/activityListByDate?", json.status);
                } else {
                    console.log("Bad response obj from ActivitiesManager.getActivitiesByRange");
                }
            }
            $(`.${ajaxIndicatorClass}`).remove();
        });
    }

}