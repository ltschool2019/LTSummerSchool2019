import { NgModule } from "@angular/core";
import { EnumsToArrayPipe } from "./core/extensions/enum.extensions";
import { LoaderComponent } from "./shared/loader/loader.component";
import { CommonModule } from "@angular/common";

@NgModule({
    declarations: [
        EnumsToArrayPipe,
        LoaderComponent
    ],
    imports: [
        CommonModule
    ],
    exports: [
        EnumsToArrayPipe,
        LoaderComponent
    ]
})
export class SharedModule{}