var logger = (typeof logger !== 'undefined' && logger) || console;
function createCombinationsBehavior(settings) {
logger.error(`>>> Entering createCombinationsBehavior(settings=${settings})`);
  var defaultSettings = {
    contentEl: false,
    fetchUrl: false
  };

  return {
    settings: $.extend({}, defaultSettings, settings),
    params: {
      availableCombinations: [],
      availableAttributeIds: []
    },

    init: function () {
	logger.error('>>> Entering init()');
      var $contentEl = $(this.settings.contentEl);
      var $attributes = $('[data-attr]', $contentEl);
      if ($attributes && $attributes.length > 0) {
        var self = this;

        $.each($attributes, function (i, attribute) {
          var id = parseInt($(attribute).data('attr'));
          self.params.availableAttributeIds.push(id);
        });

        $attributes.on('change', function () {
          self.processCombinations();
        });
      }

      this.loadCombinations();
	logger.warn('<<< Exiting init()');
    },

    loadCombinations: function () {
	logger.error('>>> Entering loadCombinations()');
      var self = this;

      $.ajax({
        cache: false,
        url: self.settings.fetchUrl,
        type: 'GET',
        success: function (response) {
		logger.error(`>>> Entering success(response=${response})`);
          self.params.availableCombinations = response;

          var selectedAttributes = self.getSelectedAttributes();
		logger.error(`success(response=${response}): selectedAttributes → ${selectedAttributes}`);
          if (selectedAttributes && selectedAttributes.length > 0) {
            // unselect selected values if they are preselected but not available
            var availableAttributes = $.grep(selectedAttributes, function (attribute) {
              return attribute.values.length > 0;
            });
            $.each(availableAttributes, function (i, attribute) {
              var combinations = self.getCombinationsByAttributeId(attribute.id, [attribute]);
              if (combinations && combinations.length > 0) {
                if (!self.isAvailableCombination(combinations, [attribute])) {
                  self.unselectValues(attribute.values);
                }
              } else {
                self.unselectValues(attribute.values);
              }
            });

          }

          self.processCombinations();
		logger.warn(`<<< Exiting success(response=${response})`);
        },
        error: function () {
		logger.error('>>> Entering error()');
          self.params.availableCombinations = [];
		logger.warn('<<< Exiting error()');
        }
      });
	logger.warn('<<< Exiting loadCombinations()');
    },

    processCombinations: function () {
	logger.error('>>> Entering processCombinations()');
      var availableAttributeIds = this.params.availableAttributeIds;
      if (availableAttributeIds.length === 0)
        return;

      var self = this;

      // disable all attribute values if combinations isn't available
      var availableCombinations = this.params.availableCombinations;
      if (availableCombinations.length === 0) {
        var valueIds = this.getAttributeValueIds();
		logger.error(`processCombinations(): valueIds → ${valueIds}`);
        $.each(valueIds, function (i, valueId) {
          self.toggleAttributeValue(valueId, false);
        });

        return;
      }

      var selectedAttributes = self.getSelectedAttributes();
	logger.error(`processCombinations(): selectedAttributes → ${selectedAttributes}`);

      $.each(availableAttributeIds, function (attrIndex, attributeId) {
        var attibuteValueIds = self.getAttributeValueIds(attributeId);
        $.each(attibuteValueIds, function (i, valueId) {
          // get only previous attributes (if any) excluding the selection by current attribute
          // and then check the combinations stock with current value
          var availableAttributes = $.grep(selectedAttributes, function (attribute, selectedAttrIndex) {
            return attrIndex > selectedAttrIndex && attribute.id !== attributeId && attribute.values.length > 0;
          });
          availableAttributes.push({ id: attributeId, values: [valueId] });

          // check if available attributes exists in combinations
          // otherwise just disable the value
          var combinations = self.getCombinationsByAttributeId(attributeId, availableAttributes);
          if (combinations && combinations.length > 0) {
            self.toggleAttributeValue(valueId, self.isAvailableCombination(combinations, availableAttributes));
          } else {
            self.toggleAttributeValue(valueId, false);
          }
        });
      });

      $(this).trigger({ type: "processed" });
	logger.warn('<<< Exiting processCombinations()');
    },

    getCombinationsByAttributeId: function (attributeId, processedAttributes) {
	logger.error(`>>> Entering getCombinationsByAttributeId(attributeId=${attributeId},processedAttributes=${processedAttributes})`);
      var availableCombinations = this.params.availableCombinations;
      if (availableCombinations.length === 0) {
        return;
      };

      return $.grep(availableCombinations, function (combination) {
        var found = $.grep(combination.Attributes, function (attribute) {
          return attribute.Id === attributeId;
        })[0];

        if (processedAttributes && processedAttributes.length > 0) {
          $.each(processedAttributes, function (i, processedAttribute) {
            found = found && $.grep(combination.Attributes, function (attribute) {
              var attrbiuteIsFound = attribute.Id === processedAttribute.id;

              // exclude unselected attribute values
              if (processedAttribute.values.length > 0) {
                $.each(processedAttribute.values, function (i, id) {
                  attrbiuteIsFound = attrbiuteIsFound && $.inArray(id, attribute.ValueIds) !== -1
                });
              }

              return attrbiuteIsFound;
            })[0];
          });
        }

	logger.warn(`<<< Exiting getCombinationsByAttributeId(attributeId=${attributeId},processedAttributes=${processedAttributes})`);
        return found;
      });
    },

    isAvailableCombination: function (combinations, attributes) {
	logger.error(`>>> Entering isAvailableCombination(combinations=${combinations},attributes=${attributes})`);
      // check if any combination have stock with specified values
      // otherwise just disable the value 
      var existedCombinations = $.grep(combinations, function (combination) {
        var valueIdsByCombinations = $.map(combination.Attributes, function (attribute) {
          return attribute.ValueIds;
        });
        var existedAttributes = $.grep(attributes, function (attribute) {
          return $.grep(attribute.values, function (valueId) {
            return $.inArray(valueId, valueIdsByCombinations) !== -1;
          })
        });

        return combination.InStock && existedAttributes.length === attributes.length;
      });

	logger.warn(`<<< Exiting isAvailableCombination(combinations=${combinations},attributes=${attributes})`);
      return existedCombinations.length > 0;
    },

    getAttributeValueIds: function (attributeId) {
	logger.error(`>>> Entering getAttributeValueIds(attributeId=${attributeId})`);
      var $contentEl = $(this.settings.contentEl);
      var $scope = attributeId ? $('[data-attr=' + attributeId + ']', $contentEl) : $contentEl;
      var $valueItems = $('[data-attr-value]', $scope);
      if ($valueItems) {
		logger.warn(`<<< Exiting getAttributeValueIds(attributeId=${attributeId})`);
        return $.map($valueItems, function (item) {
          return parseInt($(item).data('attr-value'));
        });
      }
    },

    toggleAttributeValue: function (valueId, enabled) {
	logger.error(`>>> Entering toggleAttributeValue(valueId=${valueId},enabled=${enabled})`);
      var $contentEl = $(this.settings.contentEl);
      var $value = $('[data-attr-value=' + valueId + ']', $contentEl);
      if (enabled) {
        $value.removeClass('disabled');
      } else {
        $value.addClass('disabled');
		logger.warn(`<<< Exiting toggleAttributeValue(valueId=${valueId},enabled=${enabled})`);
      }
    },

    unselectValues: function (valueIds) {
	logger.error(`>>> Entering unselectValues(valueIds=${valueIds})`);
      var $contentEl = $(this.settings.contentEl);
      $.each(valueIds, function (i, valueId) {
        var $value = $('[data-attr-value=' + valueId + ']', $contentEl);
        $value.prop('selected', false);
        $('input', $value).prop('checked', false);
        // for image square
        $value.removeClass('selected-value');
      });
	logger.warn(`<<< Exiting unselectValues(valueIds=${valueIds})`);
    },

    getSelectedAttributes: function () {
	logger.error('>>> Entering getSelectedAttributes()');
      var availableAttributeIds = this.params.availableAttributeIds;
      if (availableAttributeIds.length > 0) {
        var $contentEl = $(this.settings.contentEl);

        return $.map(availableAttributeIds, function (attributeId) {
          var selectedValues = [];

          var $attribute = $('[data-attr=' + attributeId + ']', $contentEl);
          var $attributeValues = $('[data-attr-value]', $attribute);
          if ($attributeValues && $attributeValues.length > 0) {
            $.each($attributeValues, function (i, value) {
              var $value = $(value);
              if ($value.is(':selected') || $('input', $value).is(':checked')) {
                var id = parseInt($value.data('attr-value'));
                selectedValues.push(id);
				logger.warn('<<< Exiting getSelectedAttributes()');
              }
            });
          }

          return {
            id: attributeId,
            values: selectedValues
          }
        });
      }
    }
  }
}