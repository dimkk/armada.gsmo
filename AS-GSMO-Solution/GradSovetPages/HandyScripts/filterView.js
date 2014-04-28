// SharePoint InstantListFilter - developed by Jaap Vossers http://instantlistfilter.codeplex.com/
// edited by atarutin

$(document).ready(function () {
    jQuery.extend(jQuery.expr[':'], {
        containsIgnoreCase: function (a, i, m) {
            return

            (a.textContent || a.innerText || jQuery(a).text() || '').toLowerCase().indexOf((m[3] || '').toLowerCase())

            >= 0
        }
    });

    $("table.ms-listviewtable tr.ms-vifilewheadertr").each(function () {
        if ($("td.ms-vh-group", this).size() > 0) {
            return;
        }

        var tdset = "";

        var colIndex = 0;

        $(this).children("th,td").each(function () {
            if ($(this).hasClass("ms-vh-icon")) {
                // attachment
                tdset += "<td></td>";
            }
            else {
                // filterable
                tdset += "<td><input type='text' class='vossers-filterfield' filtercolindex='" + colIndex + "' /></td>";
            }

            colIndex++;
        });

        var tr = "<tr class='vossers-filterrow'>" + tdset + "</tr>";

        $(tr).insertAfter(this);
    });


    $("input.vossers-filterfield")
		.css("border", "1px solid #7f9db9")
		.css("width", "100%")
		.css("margin", "2px")
		.css("padding", "2px")
		.keyup(function () {
		    var inputClosure = this;

		    if (window.VossersFilterTimeoutHandle) {
		        clearTimeout(window.VossersFilterTimeoutHandle);
		    }

		    window.VossersFilterTimeoutHandle = setTimeout(function () {
		        var filterValues = new Array();

		        $("input.vossers-filterfield", $(inputClosure).parents("tr:first")).each(function () {
		            if ($(this).val() != "") {
		                filterValues[$(this).attr("filtercolindex")] = $(this).val();
		            }
		        });



		        $("table.ms-listviewtable").children("tbody").each(function () {

		            $(this).children("tr").each(function () {

		                var mismatch = false;

		                $(this).children("td").each(function (colIndex) {
		                    if (mismatch) return;

		                    if (filterValues[colIndex]) {
		                        var val = filterValues[colIndex];

		                        // replace double quote character with 2 instances of itself
		                        val = val.replace(/"/g, String.fromCharCode(34) + String.fromCharCode(34));


		                        if ($(this).is(":not(:contains('" + val + "'))")) {
		                            mismatch = true;
		                        }
		                    }
		                });

		                if (mismatch) {
		                    $(this).hide();
		                }
		                else {
		                    $(this).show();
		                }
		            });
		        });

		    }, 250);
		});
});