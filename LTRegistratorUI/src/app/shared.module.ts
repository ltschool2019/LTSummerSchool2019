import { NgModule } from "@angular/core";
import { EnumsToArrayPipe } from "./core/extensions/enum.extensions";

@NgModule({
    declarations: [EnumsToArrayPipe],
    exports: [EnumsToArrayPipe]
})
export class SharedModule{}