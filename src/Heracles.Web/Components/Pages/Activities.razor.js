function deleteActivity(trackGuid) {
    if (!trackGuid)
        return;
    
    $.get("/api/Activity/Delete",
        { trackId: trackGuid },
        function (data, status) {
            if (status === "success" && data === true) {
                document.location.href="/";
            } else {
                $("h5#deleteModalLabel").text("Delete Failed");
                $("div.modal-body").text("Failed to delete activity. Please try again later.");
                $("button#deleteButton").hide();
            }
        });
}