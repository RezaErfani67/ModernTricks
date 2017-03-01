
$(document).ready(function () {

                ASQ().all(FillTask_Async(), FillPeople_Async())
                .val(function (taskResult, peopleResult) {

                    //-------------filter
                    $(".filter").StartFilter({
                        'DataSource': taskResult.data,
                        'FieldsName': ["Filter1", "Filter2", "Filter3"],
                        'rowUniqueClass': 'f1',
                        'appendToClass':'filter'
                    });




                    //---------------
                    $(".main").append(taskResult.html);

                    for (var i in taskResult.data) {
                        var s1 = "", s2 = "", s3 = "";
                        s1 = taskResult.data[i].Script;
                        s2 = taskResult.data[i].ApproveScript;
                        s3 = taskResult.data[i].RejectScript;
                        var scriptElement = document.createElement("script");

                        if (taskResult.data[i].Script != null) { s1 = taskResult.data[i].Script; } else { s1 = ""; }
                        if (taskResult.data[i].ApproveScript != null) { s2 = taskResult.data[i].ApproveScript; } else { s2 = ""; }
                        if (taskResult.data[i].RejectScript != null) { s3 = taskResult.data[i].RejectScript; } else { s3 = ""; }

                        scriptElement.type = "text/javascript";
                        scriptElement.innerHTML = s1 + " " + s2 + " " + s3;
                        $(".script_" + taskResult.data[i].ID).append(scriptElement);
                    }



                    $('.btnShowTaskLevel').on('click', function (e) {
                        e.preventDefault();
                        var $this = $(this);
                        var index = $(this).attr('index').split('_')[1];
                        var listItemID = taskResult.data[index].ListItemID;
                        var listTitle = taskResult.data[index].ListTitle;
                        window.open("http://spserver/SitePages/Shared/Task/TraceTask.aspx?ListTitle=" + listTitle + "&ListItemID=" + listItemID, '_blank');

                    });
                    $('.btnShowMessage').on('click', function (e) {
                        e.preventDefault();
                        $(".MessageDiv").remove();
                        var $this = $(this);
                        var index = $(this).attr('index').split('_')[1];

                        ASQ()
                        .seq(ShowMessage(taskResult.data[index].ID,taskResult.data[index].genericId , peopleResult.html))
                        .val(
                            function (result) {
                                $this.closest(".MainDiv").append(result);

                                //ایکس را اسکرول میکنیم تا به {وای} برسد
                                var x = $("#main");
                                var y = $("#tblMessage").offset().top;
                                $('html, body').animate({
                                    scrollTop: $("#tblMessage").offset().top -10
                                }, 1000);
                              
                            }
                            )//end val()
                    });
                    $('.btnShowDetail').click();
                    $(".container").css('width', '1317px');
                });//end val()


            });

//----------Message Function--------------
var htmlMessage = "";
var htmlPeople = "";
//----------------------------------------
var htmlTask = "";
//-----------------------------------------

//-------------------------------------------
function FillTask_Async() {
    return ASQ(function (GlobalDone) {
        ASQ()
        .then(function (done) {
            $.ajax({
                url: "/Automation/FillTask_Async",
                async: true
            })
            .success(function (result) {

                done(result);
            });


        })//end then
        .then(function (done, TaskResult) {

            function DetectDetailView(Index,tableName) {
                var htm = "";
                if (tableName == "BaniChav_ReturenedProduct") {
                    htm += "<div class='panel panel-info'>";
                    htm += "<div class=panel-heading>مرجوعی بانی چاو</div>";
                    htm += "<div class=panel-body>";
                    htm += "<table class='table table-bordered'>";
                    htm += "<tr><th>عنوان</th></tr>";
                    htm += "<tr>";
                    htm += "<td>  " + TaskResult[Index].BaniChav_ReturnedProducts_Title + "  </td>";
                    htm += "</tr>";
                    htm += "</table>";
                    htm += "</div>";
                    htm += "</div>";

                    return htm;
                }

            }

            var html = "";
            //تعریف فیلدهای مخفی--

            html += "<div class=row>";
            var backColor = "#006CB5";

            for (var i = 0; i < TaskResult.length; i++) {
                html += "<div class='MainDiv panel panel-primary' index='MainDiv_" + TaskResult[i].ID + "' Title='" + TaskResult[i].Title + "'>";
                html += "<div class=panel-heading></div>";
                html += "<table  class='table table-bordered'><tr style='color:white;background-color:" + backColor + ";border-top:solid 1px #337AB7'><th></th>";
                html += "<th>عنوان وظیفه</th>";
                html += "<th>پیام این وظیفه</th>";
                html += "<th>دلیل رد این وظیفه</th>";
                html += "</tr>";

                //Create Rows-----------------------
                html += "<tr class=row_" + TaskResult[i].ID + ">";
                html += "<div class=script_" + TaskResult[i].ID + "></div>";
                html += "<td>";

                html += "<button class=btnShowTaskLevel  title='ارسال پیام-نمایش مراحل رویه' index=btnShowTaskLevel_" + i + "><span class='glyphicon glyphicon-option-horizontal' area-hidden=true></span></button>";

                html += "<button class=btnShowMessage   title='ایجاد مکالمه' index=btnShowMessage_" + i + "><span class='glyphicon glyphicon-envelope' area-hidden=true></span></button>";
                html += TaskResult[i].ApproveHtml;
                html += TaskResult[i].RejectHtml + "</td>";
                html += "<td><strong>" + TaskResult[i].Title + "</strong></td>";
                html += "<td>" + TaskResult[i].Description + "</td>";
                html += "<td><input type=text class=txtMessage_" + TaskResult[i].ID + "></input></td>";
                html += "<tr><td  colspan=4>" + TaskResult[i].Html + "</td></tr>";
                html += "</table>";


                html += DetectDetailView(i,"BaniChav_ReturenedProduct");



                html += "</div>";//End of panel MainDiv
            }
            html += "</div>";//End of Container

            var object = {};
            object.data = TaskResult;
            object.html = html;

            GlobalDone(object);


        }
        ) // end then

    })// End ASQ


}//End function
function FillMessage_Async() {

    return ASQ(function (done) {

        $.ajax({
            url: "/Automation/FillMessage_Async",
            type: "Get",
            async: true,

        }).success(function (data) {
            var html = "<div id=tblMessage class='panel panel-success'><div class=panel-heading>مکالمات انجام شده</div>";
            html += "<table class='tblMessage table'>";
            html += "<tr>";
            html += "<th>عنوان</th>";
            html += "<th>توضیحات</th>";
            html += "<th>خوانده شده</th>";
            html += "<th>خوانده نشده</th>";

            html += "</tr>";
            for (var i = 0; i < data.length; i++) {
                var object = {};
                // object["ID"]=data[i].getAttribute("ID");
                // object["Title"]=data[i].getAttribute("Title");
                          
                if (data[i].ReadBy == null || data[i].ReadBy == "") object["ReadBy"] = ';#-'; else { object["ReadBy"] = data[i].ReadBy };
                if (data[i].UnReadBy == null || data[i].UnreadBy == "") object["UnreadBy"] = ';#-'; else { object["UnReadBy"] = data[i].UnreadBy };
                if (data[i].Description == null || data[i].Description == "") object["Description"] = '-'; else { object["Description"] = data[i].Description };
                    

                //-------------
                html += "<tr index=" + i + ">";
                          
                html += "<td>" + data[i].Title + "</td>";
                html += "<td>" + object["Description"] + "</td>";
                html += "<td>" + object["ReadBy"].split(';#')[1] + "</td>";
                html += "<td>" + object["UnreadBy"].split(';#')[1] + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            html += "</div>";


            var object = {};
            object.data = data;
            object.html = html;
            done(object);
                     
        })
    });

}
function FillPeople_Async() {

    return ASQ(function (done) {

        $.ajax({
            url: "/Automation/FillPeople_Async",
            type: "get"
        }).success(function (result) {

            var html = "<select class=txtAssignTo>";
            for (var i in result) {
                html += "<option  value='" + result[i].User_Id + "'>";//value=AccountID
                html += result[i].LFName;
                html += "</option>";
            }
            html += "</select>";

            var object = {};
            object.data = result;
            object.html = html;
            done(object);
        });

    });

}
function ShowMessage(taskId, genericId, peopleHtml) {

             
    return ASQ(function (done) {

        ASQ()
        .seq(
        FillMessage_Async(genericId))
       .val(function (messageResult) {

           var html = "<div class='MessageDiv'  style=margin:3px;>";
           if (messageResult.html != null) {
               html += messageResult.html;
                       
           }


           html += "<div class='panel panel-success'><div class=panel-heading>ایجاد پیام جدید</div>";
           html += "<div class=panel-body>";
           //----------------
           html += "<div class=col-lg-3>";
           html += "<p><b>از طرف: </b></p>";
           html += "</div>";
           html += "<div class=col-lg-9>";
           html += "<p><b>" + $(".GlobalLFName").val() + "</b></p>";
           html += "</div>";
           //-----------------
           html += "<div class=col-lg-3>";
           html += "<p>ارسال پیام به</p>";
           html += "</div>";
           html += "<div class=col-lg-9>";
           html += peopleHtml;
           html += "</div>";
           //-----------------
           html += "<div class='col-lg-3'>";
           html += "<p>متن پیام</p>";
           html += "</div>";
           html += "<div class=col-lg-9>";
           html += "<p><textarea id=Message_txtArea_"+taskId+" style=width:80%;height:100px;></textarea></p>";
           html += "</div>";
           //-----------------
           html += "<div class=col-lg-12>";
           html += "<input type=button value='ثبت' class=btnSendMessage onclick=btnSendMessage('" + taskId + "','" + genericId + "')></input>";
           html += "<input type=button value='بستن فرم' class=btnCloseMessage ></input>";
           html += "</div>";
           //-----------------
           html += "</div>";//end body
           html += "</div>";//end panel

           html += "</div>";//end MessageDiv
           done(html);

       });
    });

}
function btnSendMessage(taskId, genericId) {
               
    $.ajax({
        url: "/Automation/CreateMessage",
        type: "post",
        data: {
            Title: 'پیام اختصاصی',
            UnReadBy: $("div[index='MainDiv_" + taskId + "']").find(".txtAssignTo").val(),
            Description: $("div[index=MainDiv_" + taskId + "]").find(".MessageDiv").find("#Message_txtArea_" + taskId).val(),
            genericId: genericId,
            ForwardLink: "/Automation/TraceMessage.aspx?genericId=" + genericId
        }
    }).done(function (result) {
        notify('success', 'پیام شما با موفقیت ثبت گردید.');
        $("div[index='MainDiv_" + taskId + "']").find(".MessageDiv").slideUp(1000);

    });
}
function Notify(type, text, time, maxVisible) {

    time = time || 1000;
    maxVisible = maxVisible || 2;
    var n = noty({
        text: text,
        type: type,
        timeout: time,
        dismissQueue: true,
        layout: 'center',
        closeWith: ['click'],
        theme: 'relax',
        maxVisible: maxVisible,
        animation: {
            open: 'animated bounceInLeft',
            close: 'animated bounceOutLeft',
            easing: 'swing',
            speed: 500
        }
    });
    //console.log('html: ' + n.options.id);
}
function HidePanel(taskId) {
    var $this = $(this);
    $("[index='MainDiv_" + taskId+"']").fadeOut(1000, function () {
        $this.remove();
    })

}
function DoneTask(taskId)
{
    return ASQ(function (done) {
        $.ajax({ url: '/Automation/DoneTask', data: { taskId: taskId } }).done(function (result) { done(result);});
    });
}
function RejectTask(taskId) {
    return ASQ(function (done) {
        $.ajax({ url: '/Automation/RejectTask', data: { taskId: taskId, message: $(".txtMessage_" + taskId).val() } }).done(function (result) { done(result); });
    });
}
function ActiveTask(genericId, taskOrder) {
    return ASQ(function (done) {
        $.ajax({ url: '/Automation/ActiveTask', data: { genericId: genericId, taskOrder: taskOrder } }).done(function (result) { done(result); });
    });
}
function ChangeResponsible(genericId,taskOrder,userId) {

    return ASQ(function(done) {
        
        $.ajax({
            url: '/Automation/ChangeResponsible',
            data: { taskId:taskId , taskOrder:taskOrder,userId:userId}
        }).done(function (result) { done(result); });

    });

}

$(document).on('click', '.btnCloseMessage', function () {

    $(this).closest('.MessageDiv').slideUp();
});
