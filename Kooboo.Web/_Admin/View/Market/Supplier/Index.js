$(function() {
    var viewModel = function() {
        var self = this;

        this.pager = ko.observable();

        this.getList = function(page) {
            Kooboo.Supplier.list({
                page: page || 1
            }).then(function(res) {
                if (res.success) {
                    self.handleData(res.model);
                }
            })
        }
        this.handleData = function(data) {
            self.pager(data);

            var docs = data.list.map(function(item) {
                return {
                    id: item.id,
                    article: {
                        title: item.name,
                        description: item.description,
                        url: Kooboo.Route.Get(Kooboo.Route.Supplier.ExpertisePage, {
                            id: item.id
                        }),
                        class: "title",
                        newWindow: true
                    },
                    
                    price: {
                        text: item.symbol + item.price,
                        class: 'label-sm label-info',
                        tooltip: item.currency
                    },
                    orgName: {
                        text: item.orgName,
                        class: 'label-sm gray'
                    }
                }
            })

            var data = {
                docs: docs,
                columns: [{
                    displayName: 'Article',
                    fieldName: 'article',
                    type: 'article'
                },{
                    displayName: 'Price',
                    fieldName: 'price',
                    type: 'label',
                    showClass: 'table-short'
                }, {
                    displayName: "Supplier",
                    fieldName: "orgName",
                    type: 'label',
                    showClass: 'table-short'
                }],
                
                kbType: Kooboo.Supplier.name,
                unselectable: true
            };

            self.tableData(data);
        }

        Kooboo.EventBus.subscribe("kb/pager/change", function(page) {
            self.getList(page);
        })

        this.getList();
    }

    viewModel.prototype = new Kooboo.tableModel(Kooboo.Supplier.name);
    var vm = new viewModel();
    ko.applyBindings(vm, document.getElementById('main'));
})