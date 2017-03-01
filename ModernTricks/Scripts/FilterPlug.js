
(function ($) {
    'use strict';

    $.fn.StartFilter = function (options) {

        var GlobalDataSourceForFilter = [];
        var GlobalFieldsNameForFilterArray = [];

        var selectedFieldName = [];
        var selectedFieldValue = [];
        var randomTextForBtn = makeRandomText();

        var opts = $.extend({
            DataSource: [],
            FieldsName: [],
            rowUniqueClass: '',
            appendToClass: ''
        }, options);


        GlobalDataSourceForFilter = opts.DataSource;
        GlobalFieldsNameForFilterArray = opts.FieldsName;

       
        console.log(GlobalDataSourceForFilter);
        console.log(GlobalFieldsNameForFilterArray);

        //ساخت اولین ردیف فیلتر - سطح صفر
        var firstFieldUnique = getUnique(GlobalDataSourceForFilter, GlobalFieldsNameForFilterArray[0]);
        console.log(firstFieldUnique);
        //ساخت اولین فیلتر
        var htm = "<div class=row>";
        htm += "<div class='Level0 col-sm-12';>";
        for (var j in firstFieldUnique.keys) {
            htm += "<input class='btn btn-default " + randomTextForBtn + "' btnFieldNameForFilter='" + GlobalFieldsNameForFilterArray[0] + "' btnFieldValueForFilter='" + firstFieldUnique.keys[j] + "'  btnFieldIndexForFilter=0 type=button value='" + firstFieldUnique.keys[j] + " (" + firstFieldUnique.values[j] + ")' style='margin:2px;min-width:250px;'></input>";
        }
        htm += "</div>";
        htm += "</div>";//end row


        $("." + opts.appendToClass).append(htm);



        function getNextLevelSource(dataSource, PrevFields, PrevValues) {
            var source = dataSource;
            for (var i = 0; i < PrevValues.length; i++) {
                source = $.grep(source, function (v) {
                    console.log("V: " + v);
                    return v[PrevFields[i]] == PrevValues[i]
                });
            }
           return source;
        }
       
        function ShowElementForFilter(selectedFieldName, selectedFieldValue) {

            //تمام المنت هایی که دارای اتربیوتی به نام فیلتر هستند و  مقدارشان برابر یس هست ، مخفی می شوند.
            $("[" + opts.rowUniqueClass + "='yes']").hide();

            var s = "";
            for (var i in selectedFieldName) {

                s += "[" + selectedFieldName[i] + " = '" + selectedFieldValue[i] + "']";

            }
            //s=[selectedFieldName[0]=selectedFieldValue[0]][selectedFieldName[1]=selectedFieldValue[1]]
            $(s).show();

        }
        function getUnique(data, field) {

            var counts = {};
            for (var i = 0; i < data.length; i++)
            {
            counts[data[i][field]] = 1 + (counts[data[i][field]] || 0);
            }
            
            var keys = Object.keys(counts);
            var values = Object.keys(counts).map(function (k) { return String(counts[k]) });

            var object = {};
            object.keys = keys;
            object.values = values;
            return object;

        }
        function makeRandomText() {
            var text = "";
            var possible = "abcdefghijklmnopqrstuvwxyz";
            for (var i = 0; i < 6; i++) {
                text += possible.charAt(Math.floor(Math.random() * possible.length));

            }
            return text
        }
        function AddAttributeToTable(dataSource, fieldsName) {

        }
        $(document).on('click', "." + randomTextForBtn, function () {




            //دکمه های سطح بعد را تولید کنیم
            //برای این کار باید اول دیتا سورس جدید را بسازیم.
            //برای این کار باید مقداری که توسط باتن کلیک شده است را بدست آورده و سورس جدید را بر اساس آن فیلتر کنیم.
            //بنابراین اطلاعات دکمه ی کلیک شده ی جاری را از اتریبیوت های داخل خود دکمه،میگیریم.
            var fieldIndexForFilter = parseInt($(this).attr("btnFieldIndexForFilter"));
            var fieldNameForFilter = $(this).attr("btnFieldNameForFilter");
            var fieldValueForFilter = $(this).attr("btnFieldValueForFilter");



            console.log("GlobalFieldsNameForFilterArray: " + GlobalFieldsNameForFilterArray);
            console.log("fieldIndexForFilter: " + fieldIndexForFilter);
            console.log("fieldNameForFilter: " + fieldNameForFilter);
            console.log("fieldValueForFilter: " + fieldValueForFilter);





            //قبل از اینکه شروع کنیم به تولید فیلتر سطح بعد،باید ابتدا به پاک سازی سطوحی که قبلا ایجاد کرده ایم بپردازیم.
            //اولین کار حذف اچ تی ام ال های سطح بعد است.

            if (fieldIndexForFilter == 0) { $(".Level1").remove(); $(".Level2").remove(); $(".Level3").remove(); $(".Level4").remove(); $(".Level5").remove(); $(".Level6").remove(); }
            if (fieldIndexForFilter == 1) { $(".Level2").remove(); $(".Level3").remove(); $(".Level4").remove(); $(".Level5").remove(); $(".Level6").remove(); }
            if (fieldIndexForFilter == 2) { $(".Level3").remove(); $(".Level4").remove(); $(".Level5").remove(); $(".Level6").remove(); $(".Level7").remove(); $(".Level8").remove(); }
            if (fieldIndexForFilter == 3) { $(".Level4").remove(); $(".Level5").remove(); $(".Level6").remove(); $(".Level7").remove(); $(".Level8").remove(); }
            if (fieldIndexForFilter == 4) { $(".Level5").remove(); $(".Level6").remove(); $(".Level7").remove(); $(".Level8").remove(); }
            if (fieldIndexForFilter == 5) { $(".Level6").remove(); $(".Level7").remove(); $(".Level8").remove(); }




            //حالا باید به مدیریت سلکتد فیلد و سلکتد ولیو بپردازیم.
            //اگر عمقی که کاربر روی آن کلیک کرده،عمق 0 بود،باید آرایه ی سلکتد فیلد از ایندکس 0 به بعد(و خود ایندکس صفر) همه پاک شوند و مقدار جدید کلیک شده در آن بنشیند.
            //اگر عمقی که کاربر روی آن کلیک کرده،عمق 1 بود،باید آرایه ی سلکتد فیلد از ایندکس 1 به بعد(و خود ایندکس یک) همه پاک شوند و مقدار جدید کلیک شده در آن بنشیند.

            var leng = selectedFieldName.length;
            for (var k = parseInt(fieldIndexForFilter) ; k < leng; k++) {
                selectedFieldName.pop();
                selectedFieldValue.pop();
            }



            selectedFieldName.push(fieldNameForFilter);
            selectedFieldValue.push(fieldValueForFilter);
            console.log("selectedFieldName: " + selectedFieldName);
            console.log("selectedFieldValue: " + selectedFieldValue);


            //اینجا باید بهش بگیم و یک ایف بزاریم که میگه اگر روی دکمه ی فیلتر در آخرین سطح کلیک شده بود،بیا بی خیال شو این محاسباتو
            if (parseInt(fieldIndexForFilter) + 1 == GlobalFieldsNameForFilterArray.length)
            { }
            else
            {


                var newSource = getNextLevelSource(GlobalDataSourceForFilter, selectedFieldName, selectedFieldValue);
                //  console.log("newSource: "+newSource);

                //گرفتن فیلد یونیک خونه ی بعدی آرایه
                var getUniqDataSource = getUnique(newSource, GlobalFieldsNameForFilterArray[fieldIndexForFilter + 1]);
                // console.log("getUniqDataSource: "+getUniqDataSource.keys);



                //ایجاد اچ تی ام ال دکمه های سطح بعد
                var htm = "<div  class='Level" + (parseInt(fieldIndexForFilter) + 1) + " col-sm-12'>";
                for (var j = 0; j < getUniqDataSource.keys.length; j++) {
                    htm += "<input  type=button  class='btn btn-default " + randomTextForBtn + "' btnFieldValueForFilter='" + getUniqDataSource.keys[j] + "'  btnFieldNameForFilter='" + GlobalFieldsNameForFilterArray[fieldIndexForFilter + 1] + "' btnFieldIndexForFilter=" + Number(fieldIndexForFilter + 1) + " value='" + getUniqDataSource.keys[j] + " (" + getUniqDataSource.values[j] + ")' style='margin:2px;min-width:250px;'></input>";
                }
                htm += "</div>";


                $(this).parent().after(htm);


            }

            ShowElementForFilter(selectedFieldName, selectedFieldValue);

            //---------------------------------------خوشگل کردن دکمه ها


            //$("[btnfieldindexforfilter=0]").css('background-color', '#D1E6D3');
            //$("[btnfieldindexforfilter=1]").css('background-color', '#FBF9BE');
            //$("[btnfieldindexforfilter=2]").css('background-color', '#D7ECFE');

            $(this).closest('div').find("." + randomTextForBtn).css('background-color', '#E1E1E1').css('color', 'black').removeClass('ThisButtonWasClicked');
            $(this).css('background-color', 'rgba(255, 218, 218, 1)').css('color', 'rgba(135, 0, 0, 1)').addClass('ThisButtonWasClicked');



            //----------------------------------------




        });



    }

})(jQuery);