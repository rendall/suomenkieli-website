///<reference path="typings/jquery/jquery.d.ts" />
var SaveWord = (function () {
    function SaveWord() {
        if (SaveWord.instance != null)
            return;
        SaveWord.instance = this;
    }
    SaveWord.prototype.init = function (saveButton) {
        // jquery must be initialized. 
        // call this from onReady
        if (saveButton === void 0) { saveButton = ".saveButton"; }
        $(saveButton).click(SaveWord.instance.onSaveClick);
    };
    SaveWord.prototype.onSortSaveSuccess = function (data, textStatus, jqXHR, context) {
        console.log("onSortSaveSuccess", data, textStatus, jqXHR, context);
    };
    SaveWord.prototype.onWordSaveError = function (jqXHR, textStatus, errorThrown, context) {
        console.log("onWordSaveError:", jqXHR, textStatus, errorThrown, context);
        if (SaveWord.instance.onSaveErrorEvent != null)
            SaveWord.instance.onSaveErrorEvent(jqXHR, textStatus, errorThrown, context);
    };
    SaveWord.prototype.onWordSaveSuccess = function (data, textStatus, jqXHR, context) {
        console.log("onWordSaveSuccess", data, textStatus, jqXHR, context);
        if (SaveWord.instance.onSaveSuccessEvent != null)
            SaveWord.instance.onSaveSuccessEvent(data, textStatus, jqXHR, textStatus, context);
    };
    SaveWord.prototype.onWordSaveComplete = function (jqXHR, textStatus, context) {
        console.log("onWordSaveComplete", jqXHR, textStatus, context);
    };
    SaveWord.prototype.onWordRemoveError = function (jqXHR, textStatus, errorThrown, context) {
        console.log("onWordRemoveError:", jqXHR, textStatus, errorThrown, context);
    };
    SaveWord.prototype.onWordRemoveSuccess = function (data, textStatus, jqXHR, context) {
        console.log("onWordRemoveSuccess", data, textStatus, jqXHR, context);
    };
    SaveWord.prototype.onWordRemoveComplete = function (jqXHR, textStatus, context) {
        console.log("onWordRemoveComplete", jqXHR, textStatus, context);
        $(context).children("i").first().removeClass("glyphicon-align-justify");
        $(context).children("i").first().addClass("glyphicon glyphicon-list");
    };
    SaveWord.prototype.onSaveClick = function (e) {
        var wordToSave = $(e.currentTarget).data("word");
        SaveWord.instance.doSaveWord(wordToSave, e.currentTarget);
        if (SaveWord.instance.onSaveClick != null)
            SaveWord.instance.onSaveClickEvent(e);
    };
    SaveWord.prototype.doRemoveWord = function (wordToRemove, context) {
        if (context === void 0) { context = null; }
        var ajaxSettings = SaveWord.instance.getAjaxSettings(wordToRemove, this.vocabularyListID, context);
        ajaxSettings.error = function (jqXHR, textStatus, errorThrown) {
            SaveWord.instance.onWordRemoveError(jqXHR, textStatus, errorThrown, this);
        };
        ajaxSettings.success = function (data, textStatus, jqXHR) {
            SaveWord.instance.onWordRemoveSuccess(data, textStatus, jqXHR, this);
        };
        ajaxSettings.complete = function (jqXHR, textStatus) {
            SaveWord.instance.onWordRemoveComplete(jqXHR, textStatus, this);
        };
        ajaxSettings.method = "DELETE";
        $.ajax(ajaxSettings);
    };
    SaveWord.prototype.doSaveWord = function (wordToSave, context) {
        if (context === void 0) { context = null; }
        var ajaxSettings = SaveWord.instance.getAjaxSettings(wordToSave, this.vocabularyListID, context);
        ajaxSettings.error = function (jqXHR, textStatus, errorThrown) {
            SaveWord.instance.onWordSaveError(jqXHR, textStatus, errorThrown, this);
        };
        ajaxSettings.success = function (data, textStatus, jqXHR) {
            SaveWord.instance.onWordSaveSuccess(data, textStatus, jqXHR, this);
        };
        ajaxSettings.complete = function (jqXHR, textStatus) {
            SaveWord.instance.onWordSaveComplete(jqXHR, textStatus, this);
        };
        ajaxSettings.method = "POST";
        //if (token) {
        //    ajaxSettings.headers["X-XSRF-Token"] = token;
        //}
        $.ajax(ajaxSettings);
    };
    SaveWord.prototype.doSaveSort = function (ids) {
        var ajaxSettings = this.getAjaxSettings(null, null, null);
        ajaxSettings.data = JSON.stringify(ids);
        ajaxSettings.method = "PUT";
        ajaxSettings.success = function (data, textStatus, jqXHR) {
            SaveWord.instance.onSortSaveSuccess(data, textStatus, jqXHR, this);
        };
        $.ajax(ajaxSettings);
    };
    SaveWord.prototype.getAjaxSettings = function (wordToSave, vocabularyListID, context) {
        var ajaxSettings = {};
        ajaxSettings.url = "/api/Vocabulary";
        ajaxSettings.context = context == null ? document : context;
        ajaxSettings.contentType = "application/json;charset=utf-8",
            //ajaxSettings.success = WordInput.instance.onDataReturn;
            ajaxSettings.data = JSON.stringify({
                'word': wordToSave,
                'list': this.vocabularyListID
            });
        ajaxSettings.headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        };
        return ajaxSettings;
    };
    return SaveWord;
}());
//# sourceMappingURL=saveWord.js.map