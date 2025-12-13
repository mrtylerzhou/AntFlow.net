 export const useStore = defineStore('store', {
  state: () => ({
    userId: '',
    tableId: '',
    isTried: false,
    promoterDrawer: false,
    flowPermission1: {},
    approverDrawer: false,
    approverConfig1: {},
    copyerDrawer: false,
    copyerConfig1: {},
    conditionDrawer: false,
    conditionsConfig1: {
      conditionNodes: [],
    },
    previewDrawer: false,
    instanceViewConfig1: {},
    lowCodeFormField: {},
    approveChooseFlowNode: {},
  }),
  actions: {
    setUserId(payload) {
      this.userId = payload
    },
    setTableId(payload) {
      this.tableId = payload
    },
    setIsTried(payload) {
      this.isTried = payload
    },
    setPromoter(payload) {
      this.promoterDrawer = payload
    },
    setFlowPermission(payload) {
      this.flowPermission1 = payload
    },
    setApprover(payload) {
      this.approverDrawer = payload
    },
    setApproverConfig(payload) {
      this.approverConfig1 = payload
    },
    setCopyer(payload) {
      this.copyerDrawer = payload
    },
    setCopyerConfig(payload) {
      this.copyerConfig1 = payload
    },
    setCondition(payload) {
      this.conditionDrawer = payload
    },
    setConditionsConfig(payload) {
      this.conditionsConfig1 = payload
    },
    setPreviewDrawer(payload) {
      this.previewDrawer = payload
    },
    setPreviewDrawerConfig(payload) {
      this.instanceViewConfig1 = payload
    },
    setLowCodeFormField(payload) {
      this.lowCodeFormField = payload
    }, 
    setApproveChooseFlowNodeConfig(payload) {
      this.approveChooseFlowNode = payload
    }
  }
})
