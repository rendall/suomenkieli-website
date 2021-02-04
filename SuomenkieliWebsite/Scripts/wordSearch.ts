///<reference path="typings/jquery/jquery.d.ts" />
///<reference path="typings/jqueryui/jqueryui.d.ts" />

class WordInput {
    static instance: WordInput;
    input: HTMLInputElement;

    onEnter: any;

    onWordChange(e: JQueryEventObject) {
        //console.log("onWordChange", e);
    }



    onDefaultEnter(e: JQueryEventObject) {


        console.log("onEnter: ", e.target);

        var wordInputValue = $(e.target).val();
        if (wordInputValue == "") return;

        var ajaxSettings: JQueryAjaxSettings = {};
        ajaxSettings.url = "/api/Word/" + wordInputValue;
        ajaxSettings.context = document;
        ajaxSettings.contentType = "json";
        ajaxSettings.success = WordInput.instance.onDataReturn;

        $.ajax(ajaxSettings);

        $(this.input).autocomplete("close");
    }


    onDataReturn(e) {
        console.log("onDataReturn: ", this);

        if (e == null) e = "";

        var output: HTMLParagraphElement = <HTMLParagraphElement>document.getElementById("wordOutput");
        output.innerText = e;

        $(this.input).autocomplete("close");


    }

    onKeyPress(e: JQueryEventObject) {
        if (e.which == 13) WordInput.instance.onEnter(e);
    }

    getSuggestions(request, response) {
        jQuery.get("/api/Search/" + request.term, {}, function (data) {
            response(data);
        });



    }

    constructor(wordInputID: string, onEnterFunc = null) {
        WordInput.instance = this;
        this.input = <HTMLInputElement>document.getElementById(wordInputID);
        $(this.input).on("change paste keyup", this.onWordChange);
        $(this.input).on("keypress", this.onKeyPress);
        $(this.input).autocomplete({
            minLength:2,
            source: this.getSuggestions,
            autoFocus: false

        });

        if (onEnterFunc == null) this.onEnter = this.onDefaultEnter;
        else this.onEnter = onEnterFunc;

        $(this.input).on("autocompleteselect", this.onEnter);

    }

}

