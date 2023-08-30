variable "resource_group_name" {
  type = string
}

variable "resource_group_location" {
  type = string
}

variable "container_registry_name" {
  type = string
}

variable "container_registry_sku" {
  type = string
}

variable "app_service_plan_name" {
  type = string
}

variable "app_service_plan_kind" {
  type = string
}

variable "app_service_plan_sku_tier" {
  type = string
}

variable "app_service_plan_sku_size" {
  type = string
}

variable "app_service_plan_sku_capacity" {
  type = string
}

variable "app_service_name" {
  type = string
}

variable "app_service_https_only" {
  type = bool
}
