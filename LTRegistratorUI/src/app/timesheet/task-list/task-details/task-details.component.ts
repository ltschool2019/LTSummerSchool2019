import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProjectService } from '../../../core/service/project.service';
import { ActivatedRoute } from '@angular/router';
import { Project } from '../../../core/models/project.model';
import { Task } from '../../../core/models/task.model';
import { CustomField } from '../../../core/models/customField.model';
import { CustomValue } from '../../../core/models/customValue.model';
import { CustomFieldType } from '../../../core/models/enums/customFieldType';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { TaskService } from '../../../core/service/task.service';
import { forkJoin } from 'rxjs';
import { shareReplay } from 'rxjs/operators';

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
  private projectId: number;
  
  constructor(
    private router: Router,
    private projectService: ProjectService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private taskService: TaskService
  ) { }

  ngOnInit() {
    this.buildForm();
  }  

  private buildForm(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));
    let taskId = Number(this.route.snapshot.paramMap.get('taskId'));
    if (taskId && taskId != 0) {
      let taskRequest = this.taskService.getById(taskId);
      let projectRequest = this.projectService.getProjectDetails(this.projectId);
      forkJoin(taskRequest, projectRequest).subscribe((response: [Task, Project]) => {
        this.task = response[0];
        this.project = response[1];
        this.taskFields = this.collationFieldsWithValues(this.project.customFields, this.task.customValues);
        this.test();
      });
    } else {
      this.projectService.getProjectDetails(this.projectId).subscribe((response: Project) => {
        this.project = response;
        this.taskFields = this.collationFieldsWithValues(this.project.customFields, this.task.customValues);
        this.test();
      });
      this.task = new Task()
      this.task.name = '';      
    }
    this.taskDetailsForm = this.formBuilder.group({});
  }

  private test(): void {
    this.taskDetailsForm.addControl("TaskName", new FormControl(this.task && this.task.name ? this.task.name : '', [Validators.required, Validators.minLength(1), Validators.maxLength(200)]));
    if (this.taskFields) {
      this.taskFields.forEach((value: CustomValue, key: CustomField) => {
        let validators = [];
        if (key.isRequired || key.type == CustomFieldType.dropDown) {
          validators.push(Validators.required);
        }
        if (key.maxLength && key.maxLength > 0) {
          validators.push(Validators.maxLength(key.maxLength))
        }
        this.taskDetailsForm.addControl(key.id.toString(), new FormControl(value.value ? value.value : key.defaultValue ? key.defaultValue : "", validators));
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

  private cancel(): void {
    this.router.navigateByUrl('user/manager_projects');
  }

  private addTask(): void {
    if (this.taskDetailsForm.valid) {
      this.task.name = this.taskDetailsForm.value.TaskName;
      this.task.projectId = this.projectId;
      this.project.customFields.forEach(item => {
        let formControlValue = this.taskDetailsForm.get(`${item.id}`).value;
        let customValue = this.task.customValues.find(cv => cv.customFieldId == item.id);
        if (!customValue) {
          customValue = new CustomValue();
          customValue.type = "string";
          customValue.customFieldId = item.id;
        }
        customValue.value = formControlValue;
        this.task.customValues.push(customValue);
      });
      this.taskService.addNewTask(this.task).subscribe(
        (data): any => {
          this.router.navigateByUrl('user/manager_projects');
        },
        (err) => {

        }
      );
    }    
  }

}
