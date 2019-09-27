import { CustomFieldType } from './enums/customFieldType'
import { CustomFieldOption } from './customFieldOption.model';

export class CustomField {
    id: number;
    type: CustomFieldType;
    name: string;
    description: string;
    isRequired: boolean;
    defaultValue: string;
    maxLength: number;
    fieldOptions: Array<CustomFieldOption> = [];
}