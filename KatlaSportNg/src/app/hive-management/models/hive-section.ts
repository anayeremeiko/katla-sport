export class HiveSection {
    constructor(
        public id: number,
        public name: string,
        public code: string,
        public isDeleted: boolean,
        public storeHiveId: number,
        public lastUpdated: string
        ) {
     }
}
