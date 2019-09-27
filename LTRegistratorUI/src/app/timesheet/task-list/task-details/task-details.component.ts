import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../../../core/service/project.service';
import { ActivatedRoute } from '@angular/router';
import { Project } from '../../../core/models/project.model';
import { Task } from '../../../core/models/task.model';
import { CustomField } from '../../../core/models/customField.model';
import { CustomValue } from '../../../core/models/customValue.model';
import { CustomFieldType } from '../../../core/models/enums/customFieldType';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { fromStringWithSourceMap } from 'source-list-map';
import { KeyValuePipe } from '@angular/common';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.scss']
})
export class TaskDetailsComponent implements OnInit {
  private project: Project;
  private task: Task;
  public taskFields: Map<CustomField, CustomValue> = new Map<CustomField, CustomValue>();
  public isUpdate: boolean = false;
  private CustomFieldType = CustomFieldType;
  private taskDetailsForm: FormGroup;
  
  constructor(
    private projectService: ProjectService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit() {
    this.buildForm();
  }  

  private buildForm(): void {
    let taskId = Number(this.route.snapshot.paramMap.get('taskId'));
    if (taskId != null && taskId != 0) {
      this.isUpdate = true;
    } else {
      this.isUpdate = false;
      this.task = new Task()
      this.task.name = '';
    }
    let projectId = Number(this.route.snapshot.paramMap.get('id'));
    this.projectService.getProjectDetails(projectId)
    .subscribe((result: Project) => {
      this.project = result;
      this.taskFields = this.collationFieldsWithValues(this.project.customFields, this.task.customValues);
    });
    this.test();
  }

  private test(): void {
    this.taskDetailsForm = this.formBuilder.group({
      TaskName: [this.task && this.task.name ? this.task.name : '', [Validators.required, Validators.minLength(1), Validators.maxLength(200)]]
    });
    if (this.taskFields) {
      this.taskFields.forEach((value: CustomValue, key: CustomField) => {
        let validators = [];
        if (key.isRequired || key.type == CustomFieldType.dropDown) {
          validators.push(Validators.required);
        }
        if (key.maxLength && key.maxLength > 0) {
          validators.push(Validators.maxLength(key.maxLength))
        }
        this.taskDetailsForm.addControl(`${key.name}Control`, new FormControl(value.value ? value.value : key.defaultValue ? key.defaultValue : "", validators));
      })
    }
  }

  private collationFieldsWithValues(customFields: CustomField[], customValues: CustomValue[]) : Map<CustomField, CustomValue> {
    let result = new Map<CustomField, CustomValue>();
    customFields.forEach(customField => {
      let customValue = customValues.find(cv => cv.customFieldId == customField.id);
      if (customValue == null) {
        customValue = new CustomValue();
        customValue.id = customField.id;
        customValue.type = "string";
        customValue.value = customField.defaultValue ? customField.defaultValue : "";
      }
      result.set(customField, customValue);
    });
    
    return result;
  }

}
