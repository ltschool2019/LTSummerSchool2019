import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Project } from '../../core/models/project.model';
import { CustomField } from '../../core/models/customField.model';
import { CustomFieldType } from '../../core/models/enums/customFieldType';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { ManagerProjectsService } from '../../core/service/manager_projects.service';
import { EnumsToArrayPipe } from '../../core/extensions/enum.extensions';
import { CustomFieldOption } from '../../core/models/customFieldOption.model';
import { EmployeeService } from '../../core/service/employee.service';
import { ProjectService } from '../../core/service/project.service';
import { HttpErrorResponse } from '@angular/common/http';
import { OverlayService } from '../../shared/overlay/overlay.service';
import { ApiError } from '../../core/models/apiError.model';

@Component({
  selector: 'app-create-project',
  templateUrl: './create-project.component.html',
  styleUrls: ['./create-project.component.scss']
})
export class CreateProjectComponent implements OnInit {
  private projectId: number;

  project: Project;
  customField: CustomField;
  showAddCustomFieldPanel: boolean = false;
  projectForm: FormGroup;
  customFieldForm: FormGroup;
  customFieldTypes = CustomFieldType;
  showSpinner: boolean = false;

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private managerService: ManagerProjectsService,
    private projectService: ProjectService,
    private overlayService: OverlayService
  ) {
    this.project = new Project();
    this.customField = new CustomField();
  }

  ngOnInit() {
    this.buidForm();
    this.project.customFields.forEach(customField => {
      this.addCustomField(customField);
    });
    this.projectId = Number(window.localStorage.getItem("projectEditId"));
    if (this.projectId && this.projectId != 0) {
      this.projectService.getProjectDetails(this.projectId).subscribe(
        (project: Project) => {
          this.fillProjectData(project);
        },
        (error: HttpErrorResponse) => {
          this.overlayService.danger(error.message);
         }
      );
    }
  }

  private fillProjectData(project: Project) {
    this.project = project;
    this.projectForm.patchValue({
      ProjectName: project.name
    });
    project.customFields.forEach((item: CustomField) => {
      this.addCustomField(item);
    })
  }

  private buidForm(): void {
    this.projectForm = this.formBuilder.group({
      ProjectName: [this.project && this.project.name ? this.project.name : '', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
      CustomFields: this.formBuilder.array([])
    });

    this.customFieldForm = this.formBuilder.group({
      CustomFieldName: [this.customField && this.customField.name ? this.customField.name : '', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
      CustomFieldType: [this.customField && this.customField.type ? this.customField.type : CustomFieldType.textField, Validators.required],
      CustomFieldDescription: [this.customField && this.customField.description ? this.customField.description : '', Validators.maxLength(400)],
      CustomFieldIsRequired: [this.customField && this.customField.isRequired ? this.customField.isRequired : false],
      CustomFieldDefaultValue: [this.customField && this.customField.defaultValue ? this.customField.defaultValue : ''],
      CustomFieldMaxLength: [this.customField && this.customField.maxLength ? this.customField.maxLength : 1],
      CustomFieldOptions: [this.customField && this.customField.fieldOptions ? this.customField.fieldOptions.map(item => item.customValue).join(", ") : '']
    });

    this.customFieldForm.get('CustomFieldType').valueChanges.subscribe(item => {
      if (item == CustomFieldType.textField) {
        this.CustomFieldOptions.setValidators(null);
        this.CustomFieldMaxLength.setValidators([Validators.min(0), Validators.max(200)]);
      } else {
        this.CustomFieldMaxLength.setValidators(null);
        this.CustomFieldOptions.setValidators(Validators.required);
      }
      this.CustomFieldMaxLength.updateValueAndValidity();
      this.CustomFieldOptions.updateValueAndValidity();
    })
  }

  get ProjectName() { return this.projectForm.get('ProjectName'); }
  get CustomFieldType() { return this.projectForm.get('CustomFieldType'); }
  get CustomFieldName() { return this.customFieldForm.get('CustomFieldName'); }
  get CustomFieldDescription() { return this.customFieldForm.get('CustomFieldDescription'); }
  get CustomFieldMaxLength() { return this.customFieldForm.get('CustomFieldMaxLength'); }
  get CustomFieldOptions() { return this.customFieldForm.get('CustomFieldOptions'); }

  private addCustomField(customField: CustomField) {
    let contorl = <FormArray>this.projectForm.controls.CustomFields;
    contorl.push(
      this.formBuilder.group({
        CustomFieldId: [customField.id ? customField.id : 0],
        CustomFieldName: [customField.name],
        CustomFieldType: [customField.type],
        CustomFieldDescription: [customField.description],
        CustomFieldIsRequired: [customField.isRequired],
        CustomFieldDefaultValue: [customField.defaultValue],
        CustomFieldMaxLength: [customField.maxLength],
        CustomFieldOptions: [customField.fieldOptions ? customField.fieldOptions.map(fo => fo.customValue).join(', ') : ""]
      })
    )
  }

  private removeCustomField(index: number) {
    let control = <FormArray>this.projectForm.controls.CustomFields;
    control.removeAt(index);
  }

  private showCustomFieldForm() {
    this.showAddCustomFieldPanel = true;
  }

  private cancelCustomField(): void {
    this.customFieldForm.reset();
    this.showAddCustomFieldPanel = false;
  }

  private saveCustomField(): void {
    if (this.customFieldForm.valid) {
      var item = new CustomField();
      item.name = this.customFieldForm.value.CustomFieldName;
      item.type = this.customFieldForm.value.CustomFieldType;
      item.description = this.customFieldForm.value.CustomFieldDescription;
      item.isRequired = this.customFieldForm.value.CustomFieldIsRequired;
      if (item.type == CustomFieldType.textField) {
        item.defaultValue = this.customFieldForm.value.CustomFieldDefaultValue;
        item.maxLength = this.customFieldForm.value.CustomFieldMaxLength;
      } else {
        item.fieldOptions = this.customFieldForm.value.CustomFieldOptions.split(",").map((item: string, index: number) => {
          var result = new CustomFieldOption();
          result.customValue = item;
          result.sequence = index + 1;

          return result;
        });
      }
      this.addCustomField(item);
      this.customFieldForm.reset();
      this.showAddCustomFieldPanel = false;
    }
  }

  private cancel(): void {
    this.router.navigateByUrl('user/manager_projects');
  }

  private addProject(): void {
    if (this.projectForm.valid) {
      this.showSpinner = true;
      this.project.name = this.projectForm.value.ProjectName;
      this.project.customFields = [];
      this.projectForm.value.CustomFields.forEach(element => {
        var item = new CustomField();
        item.id = element.CustomFieldId;
        item.name = element.CustomFieldName;
        item.type = element.CustomFieldType;
        item.description = element.CustomFieldDescription ? element.CustomFieldDescription : "";
        item.isRequired = element.CustomFieldIsRequired ? element.CustomFieldIsRequired : false;
        if (item.type == CustomFieldType.textField) {
          item.defaultValue = element.CustomFieldDefaultValue;
          item.maxLength = element.CustomFieldMaxLength ? element.CustomFieldMaxLength : 0;
        } else {
          item.fieldOptions = new Array<CustomFieldOption>();
          element.CustomFieldOptions.split(',').forEach((cfo: string, index: number) => {
            var option = new CustomFieldOption();
            option.customValue = cfo;
            option.sequence = index + 1;

            item.fieldOptions.push(option);
          });
        }
        this.project.customFields.push(item);
      });
      let response;
      if (window.localStorage.getItem("projectEditId")) {
        response = this.managerService.updateManagerProject(this.project);
      } else {
        response = this.managerService.addManagerProject(this.project);
      }
      response.subscribe(
        (data: any) => {
          this.overlayService.success(window.localStorage.getItem("projectEditId") ? "Проект успешно обновлен": "Проект успешно создан");
          this.router.navigateByUrl('user/manager_projects');
        },
        (error: HttpErrorResponse) => {
          let apiError = <ApiError>error.error;
          this.overlayService.danger(apiError.message);
        }        
      ).add(() => this.showSpinner = false);
    }
  }
}
