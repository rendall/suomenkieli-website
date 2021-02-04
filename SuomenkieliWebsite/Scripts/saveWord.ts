///<reference path="typings/jquery/jquery.d.ts" />

class SaveWord {

    static instance: SaveWord;

    public vocabularyListID: Number;
    public onSaveClickEvent: any;
    public onSaveErrorEvent: any;
    public onSaveSuccessEvent: any;

    constructor() {
        if (SaveWord.instance != null) return;
        SaveWord.instance = this;

    }

    init(saveButton: string = ".saveButton") {
        // jquery must be initialized. 
        // call this from onReady


        $(saveButton).click(SaveWord.instance.onSaveClick);
    }

    onSortSaveSuccess(data: any, textStatus: string, jqXHR: JQueryXHR, context: any) {
        console.log("onSortSaveSuccess", data, textStatus, jqXHR, context);

    }

    onWordSaveError(jqXHR: JQueryXHR, textStatus: string, errorThrown: string, context: any) {
        console.log("onWordSaveError:", jqXHR, textStatus, errorThrown, context);

        if (SaveWord.instance.onSaveErrorEvent != null) SaveWord.instance.onSaveErrorEvent(jqXHR, textStatus, errorThrown, context);

    }

    onWordSaveSuccess(data: any, textStatus: string, jqXHR: JQueryXHR, context: any) {
        console.log("onWordSaveSuccess", data, textStatus, jqXHR, context);
        if (SaveWord.instance.onSaveSuccessEvent != null) SaveWord.instance.onSaveSuccessEvent(data, textStatus, jqXHR, textStatus, context);

    }

    onWordSaveComplete(jqXHR: JQueryXHR, textStatus: string, context: any) {

        console.log("onWordSaveComplete", jqXHR, textStatus, context);
    }

    onWordRemoveError(jqXHR: JQueryXHR, textStatus: string, errorThrown: string, context: any) {
        console.log("onWordRemoveError:", jqXHR, textStatus, errorThrown, context);

    }

    onWordRemoveSuccess(data: any, textStatus: string, jqXHR: JQueryXHR, context: any) {
        console.log("onWordRemoveSuccess", data, textStatus, jqXHR, context);

    }

    onWordRemoveComplete(jqXHR: JQueryXHR, textStatus: string, context: any) {
        console.log("onWordRemoveComplete", jqXHR, textStatus, context);
        $(context).children("i").first().removeClass("glyphicon-align-justify");
        $(context).children("i").first().addClass("glyphicon glyphicon-list");
    }

    onSaveClick(e: JQueryEventObject) {
        var wordToSave: string = $(e.currentTarget).data("word");
        SaveWord.instance.doSaveWord(wordToSave, e.currentTarget);
        if (SaveWord.instance.onSaveClick != null) SaveWord.instance.onSaveClickEvent(e);
    }

    doRemoveWord(wordToRemove: string, context: any = null) {
        var ajaxSettings: JQueryAjaxSettings = SaveWord.instance.getAjaxSettings(wordToRemove, this.vocabularyListID, context);

        ajaxSettings.error = function (jqXHR: JQueryXHR, textStatus: string, errorThrown: string) {
            SaveWord.instance.onWordRemoveError(jqXHR, textStatus, errorThrown, this);
        };
        ajaxSettings.success = function (data: any, textStatus: string, jqXHR: JQueryXHR) {
            SaveWord.instance.onWordRemoveSuccess(data, textStatus, jqXHR, this);
        };
        ajaxSettings.complete = function (jqXHR: JQueryXHR, textStatus: string) {
            SaveWord.instance.onWordRemoveComplete(jqXHR, textStatus, this);
        };

        ajaxSettings.method = "DELETE";
        $.ajax(ajaxSettings);
    }

    doSaveWord(wordToSave: string, context: any = null) {
        var ajaxSettings: JQueryAjaxSettings = SaveWord.instance.getAjaxSettings(wordToSave, this.vocabularyListID, context);

        ajaxSettings.error = function (jqXHR: JQueryXHR, textStatus: string, errorThrown: string) {
            SaveWord.instance.onWordSaveError(jqXHR, textStatus, errorThrown, this);
        };
        ajaxSettings.success = function (data: any, textStatus: string, jqXHR: JQueryXHR) {
            SaveWord.instance.onWordSaveSuccess(data, textStatus, jqXHR, this);
        };
        ajaxSettings.complete = function (jqXHR: JQueryXHR, textStatus: string) {
            SaveWord.instance.onWordSaveComplete(jqXHR, textStatus, this);
        };

        ajaxSettings.method = "POST";



        //if (token) {
        //    ajaxSettings.headers["X-XSRF-Token"] = token;
        //}

        $.ajax(ajaxSettings);

    }

    doSaveSort(ids: Array<Number>) {
        var ajaxSettings: JQueryAjaxSettings = this.getAjaxSettings(null, null, null);
        ajaxSettings.data = JSON.stringify(ids);
        ajaxSettings.method = "PUT";

        ajaxSettings.success = function (data: any, textStatus: string, jqXHR: JQueryXHR) {
            SaveWord.instance.onSortSaveSuccess(data, textStatus, jqXHR, this);
        };

        $.ajax(ajaxSettings);

    }

    getAjaxSettings(wordToSave, vocabularyListID, context) {
        var ajaxSettings: JQueryAjaxSettings = {};
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
        }

        return ajaxSettings;
    }
}