$(function () {
    $("#grid").jqGrid({
        url: "/Landlord/GetManageHouseLists",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['Id', 'LandlordId', 'PRTBNO', 'Address', 'IsActive'],
        colModel: [
            { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
            { key: false, hidden: true, name: 'LandlordId', index: 'LandlordId', editable: true },
            { key: false, name: 'PRTBNO', index: 'PRTBNO', editable: true, editrules: { required: true }, editoptions: {size:26}},
            { key: false, name: 'Address', index: 'Address', editable: true, editrules: { required: true }, edittype: 'textarea', editoptions: { rows: "8", cols: "26",}},
            { key: false, hidden: true, name: 'IsActive', index: 'IsActive', editable: true }
            ],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Manage House PRTBNO Details',
        emptyrecords: 'No records to display',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#pager', { edit: true, add: true, del: true, search: false, refresh: true },
        {
            // edit options
            zIndex: 100,
            autowidth: true,
            autoheight: true,
            url: '/Landlord/Edit',
            closeOnEscape: true,
            closeAfterEdit: true,
            recreateForm: true,
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            // add options
            zIndex: 100,
            autowidth: true,
           autoheight:true,
            url: "/Landlord/Create",
            closeOnEscape: true,
           closeAfterAdd: true,
            afterComplete: function (response) {
                
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        },
        {
            // delete options
            zIndex: 100,
            url: "/Landlord/Delete",
            closeOnEscape: true,
            closeAfterDelete: true,
            recreateForm: true,
            msg: "Are you sure you want to delete this task?",
            afterComplete: function (response) {
                if (response.responseText) {
                    alert(response.responseText);
                }
            }
        });
});