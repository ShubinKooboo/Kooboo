$(function() {
  var self;

  new Vue({
    el: "#app",
    data: function() {
      return {
        text: {
          domains: Kooboo.text.common.Domains,
          sites: Kooboo.text.component.breadCrumb.sites,
          dashboard: Kooboo.text.component.breadCrumb.dashboard
        },
        siteName: "",
        modalShow: false,
        domainsData: [],
        rootDomain: [],
        defaultBinding: "domain",
        tableDataSelectedRows: [],
        isSelectedDocsContainsRef: false,
        formModel: {
          domain: "",
          port: 81,
          root: ""
        },
        routerModel: {
          startPage: "",
          notFound: "",
          error: ""
        },
        formRules: {
          domain: [
            {
              pattern: /^([A-Za-z][\w\-\.]*)*$/,
              message: Kooboo.text.validation.objectNameRegex
            },
            {
              min: 1,
              max: 63,
              message:
                Kooboo.text.validation.minLength +
                0 +
                ", " +
                Kooboo.text.validation.maxLength +
                63
            },
            {
              validate: function(value) {
                var exist = _.map(self.domainsData, function(dm) {
                  return dm.fullName;
                });
                if (
                  exist.indexOf(
                    self.formModel.domain + "." + self.formModel.root
                  ) > -1
                ) {
                  return false;
                }

                return true;
              },
              message: "The domain already exists"
            }
          ],
          port: [
            { required: Kooboo.text.validation.required },
            {
              pattern: /^\d*$/,
              message: Kooboo.text.validation.invaildPort
            },
            {
              min: 0,
              max: 65535,
              message: Kooboo.text.validation.portRange
            }
          ]
        }
      };
    },
    created: function() {
      self = this;
      this.breads = [
        {
          name: this.text.sites
        },
        {
          name: this.text.dashboard
        },
        {
          name: this.text.domains
        }
      ];
      this.getDomainsData();
      this.getDomainData();
      this.getsiteNameData();
      this.initEventBus();
    },
    watch: {
      tableDataSelectedRows: function(value) {}
    },
    methods: {
      getsiteNameData: function() {
        Kooboo.Site.getName().then(function(res) {
          if (res.success) self.siteName = res.model;
        });
      },
      getDomainsData: function() {
        Kooboo.Binding.listBySite().then(function(data) {
          console.log(data.model);
          self.domainsData = data.model;
          self.domainsData[0].enableSsl = true;
          console.log(self.domainsData);
        });
      },
      showDialog: function() {
        this.modalShow = true;
      },
      cancelDialog: function() {
        this.modalShow = false;
      },
      onSave: function() {
        if (this.$refs.form.validate()) {
          Kooboo.Binding.post({
            subdomain: self.formModel.domain,
            rootdomain: self.formModel.root,
            port: self.formModel.port + "",
            defaultBinding: self.defaultBinding === "port"
          }).then(function() {
            self.getDomainsData();
            self.cancelDialog();
          });
        }
      },
      onDelete: function() {
        var confirmMessage;
        if (this.getIsSelectedDocsContainsRef(this.domainsData)) {
          confirmMessage = Kooboo.text.confirm.deleteItemsWithRef;
        } else {
          confirmMessage = Kooboo.text.confirm.deleteItems;
        }

        if (confirm(confirmMessage)) {
          var haveRelations = this.tableDataSelectedRows.some(function(s) {
            return s.relations && Object.keys(s.relations).length;
          });

          var confirmStr = haveRelations
            ? Kooboo.text.confirm.deleteItemsWithRef
            : Kooboo.text.confirm.deleteItems;

          var ids = this.tableDataSelectedRows.map(function(m) {
            return m.id;
          });

          Kooboo[Kooboo.Binding.name]
            .Deletes({
              ids: ids
            })
            .then(function(res) {
              if (res.success) {
                window.info.done(Kooboo.text.info.enable.success);
                self.getDomainsData();
                self.cancelDialog();
              } else {
                window.info.fail(Kooboo.text.info.enable.failed);
              }
            });
        }
      },
      defaultBindingCheckboxHandle: function(event) {
        this.defaultBinding = event.target.value;
      },
      getDomainData: function() {
        Kooboo.Domain.getList().then(function(res) {
          self.rootDomain = res.model;
        });
      },

      getIsSelectedDocsContainsRef: function(doc) {
        if (doc.relations) {
          var reorderedKeys = _.sortBy(Object.keys(doc.relations));
          doc.relationsTypes = reorderedKeys;
        }
        var find = _.find(doc, function(item) {
          return item.relations && Object.keys(item.relations).length;
        });

        return !!find;
      },
      initEventBus: function() {
        Kooboo.EventBus.subscribe("kb/domain/enable/domain", function(doc) {
          var find = self.domainsData.find(function(domain) {
            return domain.id == doc.id;
          });

          if (find) {
            var domain = self.rootDomain().find(function(domain) {
              return domain.id == find.domainId;
            });

            if (domain) {
              var rootDomain = domain.domainName,
                subDomain = find.subDomain;

              Kooboo.Binding.verifySSL({
                rootDomain: rootDomain,
                subDomain: subDomain
              }).then(function(res) {
                if (res.success) {
                  Kooboo.Binding.setSSL({
                    rootDomain: rootDomain,
                    subDomain: subDomain
                  }).then(function(resp) {
                    if (resp.success) {
                      getList();
                      window.info.done(Kooboo.text.info.enable.success);
                    } else {
                      window.info.fail(Kooboo.text.info.enable.failed);
                    }
                  });
                }
              });
            } else {
              window.info.fail(Kooboo.text.info.domainMissing);
            }
          }
        });
      }
    }
  });
});
