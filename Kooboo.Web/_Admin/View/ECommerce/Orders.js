$(function() {
  var self;
  new Vue({
    el: "#app",
    data: function() {
      self = this;
      return {
        breads: [
          {
            name: "SITES"
          },
          {
            name: "DASHBOARD"
          },
          {
            name: Kooboo.text.common.Orders
          }
        ],
        tableData: [],
        selected: [],
        newOrder: Kooboo.Route.Get(Kooboo.Route.Order.DetailPage),
        pager: {},
        searchKey: "",
        cacheData: null,
        isSearching: false
      };
    },
    mounted: function() {
      self.getList();
    },
    methods: {
      getList: function(page) {
        Kooboo.Order.getList({
          pageNr: page
        }).then(function(res) {
          if (res.success) {
            self.cacheData = res.model;
            self.handleData(res.model);
          }
        });
      },
      searchStart: function() {
        if (this.searchKey) {
          Kooboo.Order.search({
            keyword: self.searchKey
          }).then(function(res) {
            if (res.success) {
              self.handleData(res.model);
              self.isSearching = true;
            }
          });
        } else {
          this.isSearching = false;
          self.handleData(this.cacheData);
        }
      },
      clearSearching: function() {
        this.searchKey = "";
        this.isSearching = false;
        self.handleData(this.cacheData);
      },
      dataMapping: function(data) {
        return data.map(function(item) {
          var ob = { ...item };
          ob.createDate = new Date(item.createDate).toDefaultLangString();
          ob.Edit = {
            text: Kooboo.text.common.edit,
            url: Kooboo.Route.Get(Kooboo.Route.Order.DetailPage, {
              id: item.id
            })
          };
          return ob;
        });
      },
      handleData: function(data) {
        self.pager = data;
        self.tableData = self.dataMapping(data.list);
      },
      cancelOrder: function(data) {
        if (confirm(Kooboo.text.confirm.cancelOrder)) {
          Kooboo.Order.cancel({
            id: data.id
          }).then(function(res) {
            if (res.success) {
              data.status = 'Cancel';
              window.info.show(Kooboo.text.info.cancelOrder.success, true);
            }
          });
        }
      },
      onDelete: function() {
        if (confirm(Kooboo.text.confirm.deleteItems)) {
          var ids = self.selected.map(function(row) {
            return row.id;
          });
          Kooboo.Order.Deletes({
            ids: JSON.stringify(ids)
          }).then(function(res) {
            if (res.success) {
              self.tableData = _.filter(self.tableData, function(row) {
                return ids.indexOf(row.id) === -1;
              });
              self.selected = [];
              window.info.show(Kooboo.text.info.delete.success, true);
            }
          });
        }
      },
      changePage: function(page) {
        self.getList(page);
      }
    }
  });
});
