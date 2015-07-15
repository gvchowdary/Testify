$(function () {
    $("#grid").jqGrid({
        url: "/Tenant/GetTicketLists?status=Pending",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'TenantId', 'Title', 'Description', 'IsActive', 'CreatedDate', 'ModifiedDate', 'Status'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: false, hidden: true, name: 'TenantId', index: 'TenantId', editable: true },
            { key: false, name: 'Title', index: 'Title', editable: true, editrules: { required: true }, editoptions: { size: 26 } },
            { key: false, name: 'Description', index: 'Description', editable: true, editrules: { required: true }, edittype: 'textarea', editoptions: { rows: "8", cols: "26"} },
            { key: false, hidden: true, name: 'IsActive', index: 'IsActive', editable: true },
            { key: false,  name: 'CreatedDate', index: 'CreatedDate', editable: false },
            { key: false, name: 'ModifiedDate', index: 'ModifiedDate', editable: false },
            { key: false, name: 'Status', index: 'Status', editable: false, editoptions: { readonly: true, size: 0 } }
            ],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Pending Report Details',
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
            url: '/Tenant/Edit',
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
           url: "/Tenant/Create",
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
            url: "/Tenant/Delete",
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


    //Approved Tickets GridView
    $("#grid1").jqGrid({
        url: "/Tenant/GetTicketLists?status=Approved",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['ID', 'TenantId', 'Title', 'Description', 'IsActive', 'CreatedDate', 'ModifiedDate', 'ApprovedDate', 'Status'],
        colModel: [
            { key: true, hidden: true, name: 'ID', index: 'ID', editable: true },
            { key: false, hidden: true, name: 'TenantId', index: 'TenantId', editable: true },
            { key: false, name: 'Title', index: 'Title', editable: true, editrules: { required: true } },
            { key: false, name: 'Description', index: 'Description', editable: true, editrules: { required: true } },
            { key: false, hidden: true, name: 'IsActive', index: 'IsActive', editable: true },
            { key: false,  name: 'CreatedDate', index: 'CreatedDate', editable: true },
            { key: false, name: 'ModifiedDate', index: 'ModifiedDate', editable: true },
            { key: false, name: 'ApprovedDate', index: 'ApprovedDate', editable: true },
            { key: false, name: 'Status', index: 'Status', editable: true }
        ],
        pager: jQuery('#pager1'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: 'Approved Report Details',
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
    }).navGrid('#pager1', { edit: false, add: false, del: false, search: false, refresh: true }
        
        
        );






});