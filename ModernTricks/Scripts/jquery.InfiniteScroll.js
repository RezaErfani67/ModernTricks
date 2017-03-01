// <![CDATA[
(function ($) {
    $.fn.InfiniteScroll = function (options) {
        var defaults = {
            moreInfoDiv: '#MoreInfoDiv',
            progressDiv: '#Progress',
            loadInfoUrl: '/',
            loginUrl: '/login',
            errorHandler: null,
            completeHandler: null,
            noMoreInfoHandler: null,
            pagerSortById: "#sortArea > #pagerSortBy",
            pagerSortOrderId: "#sortArea > #pagerSortOrder",
            mainNonAjaxContentDivId: "#mainNonAjaxContent",
            paramName: "",
            pageName: "صفحه"
        };
        var options = $.extend(defaults, options);

        var showProgress = function () {
            $(options.progressDiv).css("display", "block");
        }

        var hideProgress = function () {
            $(options.progressDiv).css("display", "none");
        }

        var clearArea = function () {
            $(options.moreInfoDiv).html("");
            $(options.mainNonAjaxContentDivId).html("");
            window.scrollTo(0, 0);
        }

        return this.each(function () {
            var moreInfoButton = $(this);
            var page = 1;
            var title = document.title;
            var updatePath = function () {
                if (!$(options.pagerSortById).val()) {
                    return;
                }

                var path = "#/page/" + (page + 1) + "/" + $(options.pagerSortById).val() + "/" + $(options.pagerSortOrderId).val();
                try {
                    history.pushState({}, "", path);
                }
                catch (ex) {
                    window.location.hash = path;
                }
                document.title = title + " / " + options.pageName + " " + (page + 1);
            }

            var clickFn = function (moreInfoButton) {
                moreInfoButton.parent().hide();
                showProgress();
                var pagerSortBy = $(options.pagerSortById).val();
                var pagerSortOrder = $(options.pagerSortOrderId).val();
                $.ajax({
                    type: "POST",
                    url: options.loadInfoUrl,
                    data: JSON.stringify({ page: page, pagerSortBy: pagerSortBy, pagerSortOrder: pagerSortOrder, name: options.paramName }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    complete: function (xhr, status) {
                        var data = xhr.responseText;
                        if (xhr.status == 403) {
                            window.location = options.loginUrl;
                        }
                        else if (status === 'error' || !data) {
                            if (options.errorHandler)
                                options.errorHandler(this);
                        }
                        else {
                            if (data == "no-more-info") {
                                hideProgress();
                                moreInfoButton.parent().hide();
                                if (options.noMoreInfoHandler)
                                    options.noMoreInfoHandler(this);
                            }
                            else {
                                var $boxes = $(data);
                                var appendEl = $(options.moreInfoDiv).append(data);
                                updatePath();

                                hideProgress();
                                moreInfoButton.parent().show();
                                if (options.completeHandler)
                                    options.completeHandler(appendEl, $boxes);
                            }
                            page++;
                        }
                    }
                });
            }

            Path.map("#/page(/:page)(/:sortby)(/:order)").to(function () {
                var sortBy = this.params['sortby'] || 'date';
                var order = this.params['order'] || 'desc';
                var urlPage = parseInt(this.params['page'], 10);
                if (urlPage == page &&
                    sortBy == $(options.pagerSortById).val() &&
                    order == $(options.pagerSortOrderId).val()) {                    
                    return;
                }
                $(options.pagerSortById).val(sortBy);
                $(options.pagerSortOrderId).val(order);
                page = urlPage - 1;
                clearArea();
                clickFn(moreInfoButton);
            });
            Path.root("#/page/1/date/desc");
            if ($(options.pagerSortById).val()) {
                Path.listen();
            }

            $(options.pagerSortById + "," + options.pagerSortOrderId).change(function () {
                page = 0;
                clearArea();
                $(moreInfoButton).click();
            });

            $(moreInfoButton).click(function (event) {
                if (event.originalEvent === undefined) {
                    // triggered by code
                    page = 0;
                }
                clickFn(moreInfoButton);
            });
        });
    };
})(jQuery);
// ]]>