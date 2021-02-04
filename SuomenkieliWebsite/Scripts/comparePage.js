///<reference path="typings/jquery/jquery.d.ts" />
///<reference path="typings/jqueryui/jqueryui.d.ts" />
function onEntryClick(e) {
    var entryCell = e.target;
    var entry = entryCell.textContent.trim();
    if (entry == "")
        return;
    var isLeft = $(entryCell).hasClass("leftEntry");
    var toTextID = isLeft ? "#leftInput" : "#rightInput";
    var currInput = $(toTextID).val().trim();
    $(toTextID).val(currInput + " " + entry);
}
function onSubmitClick(e) {
    console.log("onSubmitClick");
    var leftSentence = $("#leftInput").val();
    var rightSentence = $("#rightInput").val();
    var sentenceData = {
        "fi": leftSentence,
        "en": rightSentence
    };
    var ajaxSettings = {};
    ajaxSettings.method = "POST";
    ajaxSettings.url = "/api/Sentence/Post";
    ajaxSettings.context = document;
    ajaxSettings.contentType = "application/json";
    ajaxSettings.success = onDataReturn;
    ajaxSettings.error = onDataError;
    ajaxSettings.data = JSON.stringify(sentenceData);
    $.ajax(ajaxSettings);
    $("p#sentenceStatus").text("Uploading...");
}
function onDataReturn(data, textStatus, jqXHR) {
    console.log("onDataReturn:", jqXHR.responseText, jqXHR.responseText === "");
    var reply = jqXHR.responseText === "" ? textStatus : jqXHR.responseText;
    $("#leftInput").val("");
    $("#rightInput").val("");
    $("p#sentenceStatus").text(reply);
    $("p#sentenceStatus").removeClass("error");
}
function onClearClick(e) {
    $("#leftInput").val("");
    $("#rightInput").val("");
}
function onDataError(jqXHR, textStatus, errorThrown) {
    console.log("onDataError", jqXHR, textStatus, errorThrown);
    $("p#sentenceStatus").text(errorThrown + " " + jqXHR.responseText);
    $("p#sentenceStatus").addClass("error");
}
function onCompareReady() {
    console.log("onCompareReady");
    $("td.leftEntry, td.rightEntry").on("click", onEntryClick);
    $("button#sentenceSubmit").on("click", onSubmitClick);
    $("button#clearFields").on("click", onClearClick);
}
$(document).ready(onCompareReady);
//# sourceMappingURL=comparePage.js.map